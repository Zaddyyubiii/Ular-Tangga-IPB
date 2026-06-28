Write-Host "==================================================" -ForegroundColor Green
Write-Host "  Ular Tangga IPB WebGL Auto-Builder & Runner" -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Green

$unityPath = "C:\Program Files\Unity\Hub\Editor\6000.4.8f1\Editor\Unity.exe"
$projectPath = "c:\Users\LENOVO\Desktop\Ular-Tangga-IPB"
$buildMethod = "UlarTangga.EditorSetup.WebGLBuildHelper.BuildWebGLToDocs"

# Check if Unity Editor is currently running to avoid file lock issues
$unityProcesses = Get-Process -Name "Unity" -ErrorAction SilentlyContinue
if ($unityProcesses) {
    Write-Host "[WARNING] Unity Editor terdeteksi sedang terbuka!" -ForegroundColor Yellow
    Write-Host "Batchmode Unity tidak bisa berjalan jika proyek sedang dikunci oleh Editor." -ForegroundColor Yellow
    Write-Host "Silakan tutup Unity Editor terlebih dahulu, ATAU lakukan build langsung dari dalam Unity" -ForegroundColor Yellow
    Write-Host "melalui menu: 'Ular Tangga > Build WebGL To Docs'." -ForegroundColor Yellow
    Write-Host "--------------------------------------------------"
    
    $choice = Read-Host "Apakah Anda ingin tetap mencoba memaksa build? (y/n)"
    if ($choice -ne "y") {
        Write-Host "Menjalankan server lokal langsung tanpa membangun ulang..." -ForegroundColor Green
        node server.js
        exit 0
    }
}

Write-Host "Mulai membangun (compiling) WebGL. Proses ini membutuhkan waktu beberapa menit..." -ForegroundColor Yellow
Write-Host "Harap tunggu..." -ForegroundColor DarkGray

# Detect CPU Cores for maximum multicore utilization
$cores = (Get-WmiObject -class Win32_processor).NumberOfLogicalProcessors
# Set log path
$logPath = Join-Path $projectPath "build.log"
if (Test-Path $logPath) { Remove-Item $logPath -Force }

Write-Host "Mendeteksi $cores CPU threads. Mengaktifkan kompilasi multi-core & efisiensi maksimal..." -ForegroundColor Cyan

# Start Unity in batchmode without Wait so we can track progress
$process = Start-Process -FilePath $unityPath -ArgumentList "-batchmode", "-projectPath `"$projectPath`"", "-executeMethod $buildMethod", "-job-worker-count", "$cores", "-logFile", "`"$logPath`"", "-quit" -PassThru

# Progress tracking loop
$startTime = Get-Date
$statusText = "Memulai Engine Unity..."
$percent = 0
$estimatedTotal = 60 # Default estimated total seconds

while (!$process.HasExited) {
    $elapsed = (Get-Date) - $startTime
    
    if (Test-Path $logPath) {
        $lastLines = Get-Content $logPath -Tail 15 -ErrorAction SilentlyContinue
        if ($lastLines) {
            foreach ($line in $lastLines) {
                if ($line -match "DisplayProgressbar: (.+)") {
                    $statusText = $matches[1]
                }
                if ($line -match "IL2CPP") { $percent = [math]::Max($percent, 40); $estimatedTotal = 80; $statusText = "Kompilasi C++ (IL2CPP)..." }
                if ($line -match "Emscripten") { $percent = [math]::Max($percent, 75); $statusText = "Kompilasi WebAssembly..." }
                if ($line -match "Building WebGL") { $percent = [math]::Max($percent, 90); $statusText = "Menyelesaikan Build..." }
            }
        }
    }
    
    # Smooth progress increment
    if ($percent -lt 99) { $percent += 0.2 }
    
    $remaining = [math]::Max(0, $estimatedTotal - $elapsed.TotalSeconds)
    
    Write-Progress -Activity "Building WebGL Project" -Status "[$($elapsed.ToString('mm\:ss')) berjalan | ~Maks. $([math]::Round($remaining))s tersisa] $statusText" -PercentComplete ([math]::Min(100, $percent))
    
    Start-Sleep -Milliseconds 500
}

Write-Progress -Activity "Building WebGL Project" -Completed

if ($process.ExitCode -eq 0) {
    Write-Host "[SUCCESS] WebGL berhasil dibangun dalam waktu $($((Get-Date) - $startTime).ToString('mm\:ss'))!" -ForegroundColor Green
    Write-Host "Menyatukan React UI ke dalam Build..." -ForegroundColor Yellow
    Set-Location -Path "$projectPath\web-ui"
    npm run build
    Set-Location -Path $projectPath
    Write-Host "[SUCCESS] React UI berhasil digabungkan!" -ForegroundColor Green
    
    Write-Host "--------------------------------------------------" -ForegroundColor Cyan
    Write-Host "[INFO] Build selesai! Silakan jalankan 'node server.js' secara manual jika ingin menyalakan server." -ForegroundColor Yellow
} else {
    Write-Host "[ERROR] Kompilasi WebGL gagal dengan kode keluar: $($process.ExitCode)" -ForegroundColor Red
    Write-Host "Silakan cek file build.log untuk detail error." -ForegroundColor Red
}

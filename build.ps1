Write-Host "==================================================" -ForegroundColor Green
Write-Host "  Ular Tangga IPB WebGL Auto-Builder & Runner" -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Green

$unityPath = "C:\Program Files\Unity\Hub\Editor\6000.4.8f1\Editor\Unity.exe"
$projectPath = "C:\Users\LENOVO\Desktop\Ular-Tangga-IPB"
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

# Start Unity in batchmode
$process = Start-Process -FilePath $unityPath -ArgumentList "-batchmode", "-projectPath `"$projectPath`"", "-executeMethod $buildMethod", "-quit" -Wait -NoNewWindow -PassThru

if ($process.ExitCode -eq 0) {
    Write-Host "[SUCCESS] WebGL berhasil dibangun ulang ke folder docs/!" -ForegroundColor Green
    Write-Host "Menjalankan server lokal..." -ForegroundColor Green
    Write-Host "--------------------------------------------------"
    node server.js
} else {
    Write-Host "[ERROR] Kompilasi WebGL gagal dengan kode keluar: $($process.ExitCode)" -ForegroundColor Red
    Write-Host "Pastikan tidak ada instance Unity lain yang sedang terbuka pada proyek ini." -ForegroundColor Red
}

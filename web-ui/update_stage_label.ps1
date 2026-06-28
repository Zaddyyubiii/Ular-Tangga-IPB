$file = "../Assets/Scripts/Player/PlayerEvolutionController.cs"
$content = Get-Content $file -Raw

$oldStage1 = 'case 1: return "Stage 1: Punk Bermasalah";'
$oldStage2 = 'case 2: return "Stage 2: Mahasiswa Belajar";'
$oldStage3 = 'case 3: return "Stage 3: Mahasiswa Tertib";'
$oldStage4 = 'case 4: return "Stage 4: Mahasiswa Teladan";'
$oldStage5 = 'case 5: return "Stage 5: Duta IPB University";'

$newStage1 = 'case 1: return "🎸 Punk Bermasalah";'
$newStage2 = 'case 2: return "🧢 Mahasiswa Belajar";'
$newStage3 = 'case 3: return "🎒 Mahasiswa Tertib";'
$newStage4 = 'case 4: return "👔 Mahasiswa Teladan";'
$newStage5 = 'case 5: return "👑 Duta IPB University";'

$content = $content -replace $oldStage1, $newStage1
$content = $content -replace $oldStage2, $newStage2
$content = $content -replace $oldStage3, $newStage3
$content = $content -replace $oldStage4, $newStage4
$content = $content -replace $oldStage5, $newStage5

Set-Content $file -Value $content -Encoding utf8

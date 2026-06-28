const fs = require('fs');
const file = '../Assets/Scripts/Player/PlayerEvolutionController.cs';
let content = fs.readFileSync(file, 'utf8');

content = content.replace('case 1: return "Stage 1: Punk Bermasalah";', 'case 1: return "🎸 Punk Bermasalah";');
content = content.replace('case 2: return "Stage 2: Mahasiswa Belajar";', 'case 2: return "🧢 Mahasiswa Belajar";');
content = content.replace('case 3: return "Stage 3: Mahasiswa Tertib";', 'case 3: return "🎒 Mahasiswa Tertib";');
content = content.replace('case 4: return "Stage 4: Mahasiswa Teladan";', 'case 4: return "👔 Mahasiswa Teladan";');
content = content.replace('case 5: return "Stage 5: Duta IPB University";', 'case 5: return "👑 Duta IPB University";');

fs.writeFileSync(file, content, 'utf8');

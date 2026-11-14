import os
import shutil

repo = r'C:\Unity projects\Undead_Harvest'
base_diag = r'C:\Users\User\.gemini\antigravity\brain\cd411519-42dd-4a01-ae3e-915e7e9edda9\scratch\extracted'

jar = r'C:\Users\User\.vscode\extensions\jebbs.plantuml-2.18.1\plantuml.jar'
os.system(f'java -jar "{jar}" -charset UTF-8 -tpng -o "{base_diag}\\diagrams\\png" "{base_diag}\\diagrams\\puml\\*.puml"')
os.system(f'java -jar "{jar}" -charset UTF-8 -tpng -o "{base_diag}\\usecase_seq\\png" "{base_diag}\\usecase_seq\\puml\\*.puml"')

diag_zip = os.path.join(repo, 'diagrams_updated.zip')
if os.path.exists(diag_zip):
    os.remove(diag_zip)
shutil.make_archive(diag_zip.replace('.zip', ''), 'zip', os.path.join(base_diag, 'diagrams'))
print('Updated diagrams_updated.zip')

seq_zip = os.path.join(repo, 'uml_usecase_and_sequence_updated.zip')
if os.path.exists(seq_zip):
    os.remove(seq_zip)
shutil.make_archive(seq_zip.replace('.zip', ''), 'zip', os.path.join(base_diag, 'usecase_seq'))
print('Updated uml_usecase_and_sequence_updated.zip')

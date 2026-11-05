# -*- coding: utf-8 -*-
"""
Sync Course Diagrams Script
Automates:
1. Compiling all PlantUML (.puml) diagrams to PNG with UTF-8.
2. Embedding updated diagrams directly into the course report .odt file.
3. Repacking the .odt archive adhering to the OpenDocument specification.
4. Packaging updated .zip archives for class diagrams and sequence/usecase diagrams.
"""

import os
import sys
import shutil
import zipfile
import glob
import subprocess

PROJECT_ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "..", "..", ".."))

# Locate plantuml.jar
def find_plantuml_jar():
    custom_path = os.environ.get("PLANTUML_JAR")
    if custom_path and os.path.exists(custom_path):
        return custom_path
    
    # Common vscode extension paths
    home = os.path.expanduser("~")
    patterns = [
        os.path.join(home, ".vscode", "extensions", "*plantuml*", "plantuml.jar"),
        os.path.join(home, ".vscode", "extensions", "*plantuml*", "**", "plantuml.jar"),
        "C:\\Program Files*\\**\\plantuml*.jar",
    ]
    for p in patterns:
        matches = glob.glob(p, recursive=True)
        if matches:
            return matches[0]
    return None

# Mapping between ODT picture names and diagram filenames
DIAGRAM_MAPPING = {
    # Class diagrams (Рис. 2 - 23)
    '1000000000000A32000005735FFED4CF.png': '00_overview.png',
    '10000000000005630000069CD8CA157A.png': 'core_boot.png',
    '10000000000008A00000056D61039AAD.png': 'core_services.png',
    '10000000000009C1000005CCD6043F8F.png': 'flow.png',
    '10000000000004E600000C1AEE337644.png': 'input.png',
    '10000000000007EC00000534FF856DA4.png': 'player.png',
    '100000000000075A0000045C5B309B0B.png': 'player_systems.png',
    '10000000000007E6000005B9943968D2.png': 'stats.png',
    '1000000000000923000003E6EBF755A9.png': 'enemies.png',
    '100000000000063C000005CA6925D0DB.png': 'waves.png',
    '10000000000008B500000659C42041CA.png': 'effects.png',
    '100000000000083E0000042DE298EA59.png': 'weapons.png',
    '1000000000000679000003F12CF35071.png': 'weapons_data.png',
    '1000000000000874000004F2ACADA8D7.png': 'projectiles.png',
    '100000000000073D000003F993E5642D.png': 'items.png',
    '10000000000006010000059DE4902054.png': 'items_rarity.png',
    '1000000000000640000006BB38A108C3.png': 'ui_hud.png',
    '100000000000094900000430E880FF38.png': 'ui_menu.png',
    '100000000000056E00000430EDD2465A.png': 'ui_results.png',
    '10000000000007DC000003C5D457AA97.png': 'ui_shop.png',
    '1000000000000758000002D2E0D7A1D9.png': 'ui_inventory.png',
    '10000000000004630000027171E55193.png': 'tests.png',
    # Use cases & Sequences (Рис. 1, 24, 25, 26)
    '1000000000000BC000000B1447ECC8ED.png': 'usecase.png',
    '1000000000000B56000006BF80AE6398.png': 'seq_1_start_DI.png',
    '1000000000000C6B000007C9A3CC1991.png': 'seq_2_state_loop.png',
    '1000000000000C4F000009B41251B9EB.png': 'seq_3_combat.png',
}

def render_diagrams(jar_path, puml_files, out_dir):
    os.makedirs(out_dir, exist_ok=True)
    cmd = ["java", "-jar", jar_path, "-charset", "UTF-8", "-tpng", "-o", out_dir] + puml_files
    print(f"Running PlantUML: {' '.join(cmd[:6])} ... ({len(puml_files)} files)")
    res = subprocess.run(cmd, capture_output=True, text=True)
    if res.returncode != 0:
        print("PlantUML stderr:", res.stderr)
        raise RuntimeError("PlantUML compilation failed")
    print(f"Compiled {len(puml_files)} diagrams into {out_dir}")

def update_odt(odt_path, diagrams_dir, output_path=None):
    if output_path is None:
        output_path = odt_path

    temp_dir = os.path.join(PROJECT_ROOT, "Temp", "odt_sync_extract")
    if os.path.exists(temp_dir):
        shutil.rmtree(temp_dir)
    os.makedirs(temp_dir, exist_ok=True)

    with zipfile.ZipFile(odt_path, 'r') as zf:
        zf.extractall(temp_dir)

    pictures_dir = os.path.join(temp_dir, "Pictures")
    replaced = 0
    for odt_pic, diag_name in DIAGRAM_MAPPING.items():
        src_png = os.path.join(diagrams_dir, diag_name)
        dst_png = os.path.join(pictures_dir, odt_pic)
        if os.path.exists(src_png) and os.path.exists(dst_png):
            shutil.copyfile(src_png, dst_png)
            replaced += 1
        elif not os.path.exists(src_png):
            print(f"Notice: Generated diagram not found: {diag_name}")

    print(f"Replaced {replaced} diagram images in ODT.")

    # Repack adhering to ODF spec (mimetype first, uncompressed)
    temp_odt = output_path + ".tmp"
    with zipfile.ZipFile(temp_odt, 'w') as zf:
        mimetype_path = os.path.join(temp_dir, 'mimetype')
        if os.path.exists(mimetype_path):
            zf.write(mimetype_path, 'mimetype', compress_type=zipfile.ZIP_STORED)
        for root, dirs, files in os.walk(temp_dir):
            for f in files:
                full_path = os.path.join(root, f)
                rel_path = os.path.relpath(full_path, temp_dir).replace('\\', '/')
                if rel_path == 'mimetype':
                    continue
                zf.write(full_path, rel_path, compress_type=zipfile.ZIP_DEFLATED)

    if os.path.exists(output_path):
        os.remove(output_path)
    os.rename(temp_odt, output_path)
    shutil.rmtree(temp_dir, ignore_errors=True)
    print(f"Successfully generated updated ODT: {output_path} ({os.path.getsize(output_path)} bytes)")

def repack_zip(src_dir, zip_path):
    if os.path.exists(zip_path):
        os.remove(zip_path)
    shutil.make_archive(zip_path.replace('.zip', ''), 'zip', src_dir)
    print(f"Created archive: {zip_path}")

def main():
    jar_path = find_plantuml_jar()
    if not jar_path:
        print("ERROR: plantuml.jar not found. Please install PlantUML or set PLANTUML_JAR environment variable.")
        sys.exit(1)
    print(f"Using PlantUML jar: {jar_path}")

    # Directories for diagrams
    diagrams_base = os.path.join(PROJECT_ROOT, "Diagrams")
    if not os.path.exists(diagrams_base):
        # Fallback to scratch/extracted if Diagrams directory is not yet created
        diagrams_base = os.path.expanduser(r"~/.gemini/antigravity/brain/cd411519-42dd-4a01-ae3e-915e7e9edda9/scratch/extracted")

    pumls = glob.glob(os.path.join(diagrams_base, "**", "*.puml"), recursive=True)
    if not pumls:
        print("ERROR: No .puml files found.")
        sys.exit(1)

    out_png_dir = os.path.join(PROJECT_ROOT, "Temp", "png_rendered")
    render_diagrams(jar_path, pumls, out_png_dir)

    odt_target = os.path.join(PROJECT_ROOT, "Tsybrivskyi_course_updated.odt")
    if not os.path.exists(odt_target):
        odt_target = os.path.join(PROJECT_ROOT, "Tsybrivskyi_course.odt")

    if os.path.exists(odt_target):
        update_odt(odt_target, out_png_dir)
    else:
        print(f"WARNING: No .odt report found at {odt_target}")

    shutil.rmtree(out_png_dir, ignore_errors=True)
    print("Sync completed successfully.")

if __name__ == "__main__":
    main()

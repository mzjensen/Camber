import os
import re

def extract_migrations(file_path):
    migrations = []
    
    # Regular expression to find the pattern
    pattern = re.compile(r'\[NodeMigration\(\s*"([^"]+)",\s*"([^"]+)"\s*\)\]', re.MULTILINE)

    with open(file_path, 'r') as file:
        content = file.read()
        matches = pattern.findall(content)
        
        for old_name, new_name in matches:
            migrations.append((old_name, new_name))
            print(f"Found migration: oldName = '{old_name}', newName = '{new_name}'")
    
    return migrations

def scan_directory(directory):
    all_migrations = []
    
    # Walk through the directory and find .cs files
    for root, _, files in os.walk(directory):
        for file in files:
            if file.endswith(".cs"):
                file_path = os.path.join(root, file)
                print(f"Scanning file: {file_path}")
                migrations = extract_migrations(file_path)
                all_migrations.extend(migrations)
    
    return all_migrations

def generate_xml(migrations, output_file):
    with open(output_file, 'w') as file:
        # Write the XML header
        file.write('<?xml version="1.0" ?>\n')
        file.write('<migrations>\n')
        
        for old_name, new_name in migrations:
            file.write('  <priorNameHint>\n')
            file.write(f'    <oldName>{old_name}</oldName>\n')
            file.write(f'    <newName>{new_name}</newName>\n')
            file.write('  </priorNameHint>\n')
        
        file.write('</migrations>\n')

# Set directories
script_dir = os.path.dirname(os.path.abspath(__file__))
proj_directory = os.path.join(script_dir, '..', 'Camber')
output_dir = os.path.join(script_dir, '..', '..', 'build', 'Debug')
output_xml = os.path.join(output_dir, "Camber.Migrations.xml")

# Scan project files
migrations = scan_directory(proj_directory)

# Generate the XML file
generate_xml(migrations, output_xml)

print(f"Migration XML saved to {output_xml}")

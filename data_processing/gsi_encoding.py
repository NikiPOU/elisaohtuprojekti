import json

class DataEncoding:

    def create_json_file(self, data, file_name):
        json_data = json.dumps(data)
        with open(f"{file_name}", "w") as file:
            file.write(json_data)

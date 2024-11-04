import urllib.request
import urllib.error
import json
from gsi_processor import DataProcessor

class DataReceiver:
    def __init__(self):
        self.gsi_data = None
        self.data_processor = DataProcessor()

    def get_gsi_data(self, uri):
        while True:
            try:
                request_result = urllib.request.urlopen(uri)
                self.gsi_data = request_result.read()
                all_player_data = json.loads(self.gsi_data)
                self.data_processor.parse_movement_data_live(all_player_data)
                self.data_processor.parse_statistics_live(all_player_data)

            except urllib.error.URLError:
                print("Error: Cannot connect to requested server")
    
    def return_gsi_data(self):
        if self.gsi_data is not None:
            return self.gsi_data
        else:
            return "No data available"


if __name__ == "__main__":
    receiver = DataReceiver()
    receiver.get_gsi_data("http://localhost:3000/data")
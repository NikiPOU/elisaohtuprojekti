from gsi_processor import DataProcessor

class DataReceiver:
    def __init__(self):
        self.data_processor = DataProcessor()

    def get_gsi_data(self, data):
        all_player_data = data
        self.data_processor.parse_movement_data_live(all_player_data)
        self.data_processor.parse_statistics_live(all_player_data)

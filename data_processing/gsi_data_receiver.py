from gsi_processor import DataProcessor

class DataReceiver:
    def __init__(self):
        self.data_processor = DataProcessor()

    def get_gsi_data(self, data):
        self.data_processor.parse_data(data)

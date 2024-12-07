from gsi_processor import DataProcessor

class DataReceiver:
    '''A class that receives and processes GSI data.
    Attributes: 
        DataProcessor: 
            An instance of the DataProcessor class is used to parse 
            and process incoming gsi data.
    '''
    def __init__(self):
        '''Creates an instance of the DataProcessor class to 
        handle parsing and processing of data.
        '''
        self.data_processor = DataProcessor()

    def get_gsi_data(self, data):
        '''Receives and process GSI data.
        Args: 
        data: 
            GSI data that is processed by the DataProcessor.
        '''

        self.data_processor.parse_data(data)

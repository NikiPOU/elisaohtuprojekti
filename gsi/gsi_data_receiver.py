import urllib.request
import urllib.error

class DataReceiver:
    def __init__(self, uri):
        self.uri = uri
        self.gsi_data = None

    def get_gsi_data(self):
        while True:
            try:
                request_result = urllib.request.urlopen(self.uri)
                self.gsi_data = request_result.read()
                print(self.gsi_data)

            except urllib.error.URLError:
                print("Error: Cannot connect to requested server")
    
    def return_gsi_data(self):
        if self.gsi_data is not None:
            return self.gsi_data
        else:
            return "No data available"


if __name__ == "__main__":
    receiver = DataReceiver("http://localhost:3000/data")
    receiver.get_gsi_data()
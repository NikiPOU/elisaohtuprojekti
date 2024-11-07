from flask import Flask
from gsi_data_receiver import DataReceiver

app = Flask(__name__)
data_receiver = DataReceiver()

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=3000)

import routes
from flask import Flask, request, jsonify
from gsi_data_receiver import DataReceiver
from db import create_database

app = Flask(__name__)
data_receiver = DataReceiver()

@app.route('/', methods=['POST'])
def receive_data():
    try:
        latest_data = request.get_json()
        data_receiver.get_gsi_data(latest_data)
        return jsonify(status="success"), 200
    except Exception as e:
        print(f"Error processing data: {e}")
        return jsonify(status="error", error=str(e)), 500

if __name__ == '__main__':
    create_database()
    app.run(host='0.0.0.0', port=3000)
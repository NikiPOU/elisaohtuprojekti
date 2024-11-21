from app import app
from flask import request, jsonify
from gsi_data_receiver import DataReceiver

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
    
@app.route('/statistics', methods=['GET'])
def get_statistics():
    try:
        with open("statistics.json", "r") as file:
            statistics = file.read()
        return jsonify(statistics), 200
    except Exception as e:
        print(f"Error getting statistics: {e}")
        return jsonify(status="error", error=str(e)), 500

@app.route('/player_positions', methods=['GET'])
def get_positions():
    try:
        with open("player_positions.json", "r") as file:
            statistics = file.read()
        return jsonify(statistics), 200
    except Exception as e:
        print(f"Error getting statistics: {e}")
        return jsonify(status="error", error=str(e)), 500

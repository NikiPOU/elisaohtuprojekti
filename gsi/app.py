from flask import Flask, request, jsonify

app = Flask(__name__)

latest_data = {}

@app.route('/', methods=['POST'])
def index():
    global latest_data
    latest_data = request.get_json()
    return jsonify(status="success"), 200

@app.route('/data', methods=['GET'])
def get_data():
    return jsonify(latest_data)

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=3000)
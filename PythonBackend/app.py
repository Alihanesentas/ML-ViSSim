from flask import Flask, jsonify, request
from services import CostSurfaceService, SimulationService
from models import LinearRegression 
import numpy as np


app = Flask(__name__)

# --- Initialize Services and Models ---
cost_service = CostSurfaceService(resolution=20)
sim_service = SimulationService()
models_repo = {
    "linear_regression": LinearRegression()
}
data_store = {
    "default_data": "TODO: Load data here"
}
# --------------------------------------

@app.route("/get_cost_surface")
def api_get_cost_surface():
    model_name = request.args.get("model", "linear_regression")
    data_id = request.args.get("data_id", "default_data")
    
    model = models_repo[model_name]
    data = data_store[data_id]
    surface_data = cost_service.get_surface(model, data)
    return jsonify(surface_data)



@app.route("/calculate_next_step", methods=['POST'])
def api_calculate_next_step():
    json_data = request.get_json()
    model = models_repo[json_data["model"]]
    data = data_store[json_data["data_id"]]
    
    current_w = np.array(json_data["current_w"])
    algorithm = json_data["algorithm"]
    lr = json_data.get("learning_rate", 0.01)
    
    step_data = sim_service.get_next_step(model, data, current_w, algorithm, lr)
    return jsonify(step_data)

if __name__ == "__main__":
    app.run(port=5000)

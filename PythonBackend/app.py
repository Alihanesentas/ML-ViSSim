from flask import Flask, jsonify, request
# Imports are now cleaner thanks to __init__.py
from services import CostSurfaceService, SimulationService, DataService 
from models import LinearRegression
from algorithms import GradientDescent, BruteForce # <-- New
import numpy as np

app = Flask(__name__)

# --- Initialize Services and Repositories ---
data_service = DataService()
cost_service = CostSurfaceService(resolution=20)
sim_service = SimulationService()
models_repo = {
    "linear_regression": LinearRegression()
}
# A "factory" for creating algorithm objects
algorithms_repo = {
    "GradientDescent": GradientDescent,
    "BruteForce": BruteForce
}
# --------------------------------------

@app.route("/get_cost_surface")
def api_get_cost_surface():
    model_name = request.args.get("model", "linear_regression")
    data_id = request.args.get("data_id", "default_data")
    
    model = models_repo.get(model_name)
    data = data_service.get_data(data_id)
    
    if not model or not data:
        return jsonify({"error": "Model or data not found"}), 404
        
    surface_data = cost_service.get_surface(model, data)
    return jsonify(surface_data)

@app.route("/calculate_next_step", methods=['POST'])
def api_calculate_next_step():
    json_data = request.get_json()
    
    # 1. Get Model and Data
    model = models_repo.get(json_data["model"])
    data = data_service.get_data(json_data["data_id"])

    # 2. Get State
    current_w = np.array(json_data["current_w"])
    hyperparams = json_data.get("hyperparameters", {}) #
    
    # 3. Get the *Class* for the requested algorithm
    algo_class = algorithms_repo.get(json_data["algorithm"])
    
    if not model or not data or not algo_class:
        return jsonify({"error": "Invalid model, data, or algorithm"}), 404
    
    # 4. Create an *instance* of the algorithm with its hyperparameters
    try:
        algorithm_instance = algo_class(hyperparameters=hyperparams)
    except Exception as e:
        return jsonify({"error": f"Failed to init algorithm: {e}"}), 400

    # 5. Call the simulation service
    step_data = sim_service.get_next_step(model, algorithm_instance, data, current_w)
    
    return jsonify(step_data)

if __name__ == "__main__":
    app.run(port=5000)
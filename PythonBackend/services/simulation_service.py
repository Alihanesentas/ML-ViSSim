import numpy as np

# This is the "Light Load" service.
# Its only job is to calculate *one* new step for the moving point.
class SimulationService:
    
    def get_next_step(self, model, data, current_w, algorithm_name, learning_rate):
        """
        Calculates and returns the *next* simulation step.
        """
        print(f"Python: Calculating next step for {algorithm_name}...")
        
        if algorithm_name == "GradientDescent":
            # 1. Call the model's gradient function
            (new_w, new_cost) = model.calculate_gradient(data, current_w, learning_rate)
            # 2. Return the single new point
            return {"w": new_w, "cost": new_cost}
        
        elif algorithm_name == "BruteForce":
            # TODO:
            # 1. Pick the *next* point on the grid (e.g., current_index + 1)
            # 2. Calculate its cost: new_cost = model.calculate_cost(data, new_w)
            # 3. return {"w": new_w, "cost": new_cost}
            pass
        
        # Fallback / Mock
        return {"w": [0,0], "cost": 0}
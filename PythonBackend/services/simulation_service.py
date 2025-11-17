import numpy as np

# "Light Load" service
class SimulationService:
    
    def get_next_step(self, model, algorithm, data, current_w):
        """
        This method is now a simple "router".
        It doesn't know *how* the step is calculated, only
        that the algorithm object has a .step() method.
        """
        print(f"Python: Routing to algorithm.step()...")
        
        # This is Polymorphism:
        (new_w, new_cost) = algorithm.step(model, data, current_w)
        
        return {"w": new_w, "cost": new_cost}
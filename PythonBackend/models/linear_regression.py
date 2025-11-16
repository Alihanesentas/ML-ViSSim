import numpy as np
from models.base_model import BaseModel

# This is our first *concrete* model, implementing the BaseModel contract.
class LinearRegression(BaseModel):
    
    def calculate_cost(self, data, w):
        """
        Calculates the Mean Squared Error (MSE) for Simple Linear Regression.
        w[0] is w0 (intercept), w[1] is w1 (slope).
        """
        # TODO:
        # 1. Get x and y from 'data'
        # 2. Calculate y_pred = w[0] + w[1] * x
        # 3. Calculate cost = np.mean((y_pred - y)**2)
        # 4. return cost
        
        print("Python: Calculating Cost...")
        return 5.0 # Placeholder (mock) value

    def calculate_gradient(self, data, w, learning_rate):
        """
        Calculates the partial derivatives for w0 and w1 and updates them.
        """
        # TODO:
        # 1. Get x and y from 'data'
        # 2. Calculate y_pred = w[0] + w[1] * x
        # 3. Calculate derivatives (gradients) dW0 and dW1
        # 4. new_w0 = w[0] - learning_rate * dW0
        # 5. new_w1 = w[1] - learning_rate * dW1
        # 6. new_cost = self.calculate_cost(data, [new_w0, new_w1])
        # 7. return ([new_w0, new_w1], new_cost)
        
        print("Python: Calculating Gradient...")
        new_w = [w[0] * 0.9, w[1] * 0.9] # Placeholder (mock) logic
        new_cost = 4.5 # Placeholder (mock) value
        return (new_w, new_cost)
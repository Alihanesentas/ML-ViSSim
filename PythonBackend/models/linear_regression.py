import numpy as np
from models.base_model import BaseModel

# This is our first *concrete* model, implementing the new BaseModel contract.
class LinearRegression(BaseModel):
    
    def calculate_cost(self, data, w):
        # This implementation is based on our discussion
        x = data["x"].ravel() # 1D (N,) array
        y = data["y"]         # 1D (N,) array
        n_samples = len(y)
        
        # y_pred = w0 + w1*x
        y_pred = w[0] + (w[1] * x)
        
        # Cost = 1/2N * sum( (y_pred - y)^2 )
        cost = np.sum((y_pred - y)**2) / (2 * n_samples)
        return cost

    def calculate_gradient_at_point(self, data, w):
        # This implementation is based on our discussion
        x = data["x"].ravel()
        y = data["y"]
        n_samples = len(y)
        
        # y_pred = w0 + w1*x
        y_pred = w[0] + (w[1] * x)
        
        # Error vector
        errors = y_pred - y
        
        # Partial derivatives (gradients)
        dw0 = (1 / n_samples) * np.sum(errors)
        dw1 = (1 / n_samples) * np.dot(x, errors)
        
        return np.array([dw0, dw1])
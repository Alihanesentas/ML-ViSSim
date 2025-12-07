# This is the "contract" or abstract class.
# All models (LinearRegression, PolynomialRegression, etc.) MUST implement this.
from abc import ABC, abstractmethod
import numpy as np
class BaseModel(ABC):
    
    @abstractmethod
    def calculate_cost(self, data, w):
        """
        Calculates the cost (e.g., MSE) for a given weight vector 'w'.
        This is a "stateless" method, used by the CostSurfaceService.
        """
        pass
    @abstractmethod
    def calculate_gradient_at_point(self, data, w):
        """
        Calculates the *gradient* (partial derivatives) at point 'w'.
        It does *not* apply the learning rate or calculate the next step.
        It just returns the "slope" of the terrain at that point.
        """
        pass
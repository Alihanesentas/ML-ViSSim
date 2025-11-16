# This is the "contract" or abstract class.
# Every new model we add (Polynomial, Ridge, etc.) MUST follow this structure.
from abc import ABC, abstractmethod

class BaseModel(ABC):
    
    @abstractmethod
    def calculate_cost(self, data, w):
        """
        Calculates the total cost (e.g., MSE) for the given dataset 'data'
        using the provided weight vector 'w' (e.g., [w0, w1]).
        
        This is used by the CostSurfaceService to draw the "bowl".
        """
        pass

    @abstractmethod
    def calculate_gradient(self, data, w, learning_rate):
        """
        Calculates a single step of Gradient Descent.
        Takes the current weights 'w' and returns the *new* weights
        and the *new* cost after taking one step.
        """
        pass
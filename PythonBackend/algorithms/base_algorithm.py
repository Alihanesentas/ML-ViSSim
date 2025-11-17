from abc import ABC, abstractmethod

# This is the "contract" for all our algorithms (Strategy Pattern).
class BaseAlgorithm(ABC):
    
    def __init__(self, hyperparameters):
        """Stores the hyperparameters for this algorithm instance."""
        self.hyperparameters = hyperparameters
    
    @abstractmethod
    def step(self, model, data, current_w):
        """
        Takes the model, data, and current position (current_w),
        and returns the *next* position and cost: (new_w, new_cost).
        """
        pass
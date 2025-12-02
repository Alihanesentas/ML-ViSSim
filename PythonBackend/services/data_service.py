from sklearn.datasets import make_regression
from sklearn import datasets
from sklearn.preprocessing import StandardScaler
import uuid # Added for generating unique data IDs

class DataService:
    def __init__(self):
        # The data_store (data repository) is now inside this class
        self.data_store = {
            "default_data": self._load_initial_data()
        }
        print("DataService initialized and default data loaded.")

    def _load_initial_data(self):
        X,y = make_regression(n_samples=3000,n_features=1,noise=40,bias=1,random_state=42)
        X_features = X.tolist()
        scalar_x = StandardScaler()
        scalar_y = StandardScaler()
        X_scaled = scalar_x.fit_transform(X_features)
        y_scaled = scalar_y.fit_transform(y.reshape(-1,1)).ravel()
        print(f"Data Loaded: Synthetic Regression (Noise: 40.0). Shape: {X_scaled.shape}")
        return {"x": X_scaled.tolist(), "y": y_scaled, "scalers": (scalar_x,scalar_y)}
    
                
    def get_data(self, data_id):
        # This is the public method app.py will call to get data
        return self.data_store.get(data_id)
    
    def load_user_data(self, file_content):
        # TODO (Future): This method will process the data imported
        # by the user (from the 'import data' button) and add it
        # to self.data_store with a new, unique ID.
        # It will return the new data_id.
        
        print(f"TODO: Processing user file content...")
        new_id = f"user_{uuid.uuid4().hex[:6]}"
        # self.data_store[new_id] = ... (processing logic)
        return new_id

    # --- NEW METHOD YOU ASKED FOR ---
    def generate_synthetic_data(self, params):
        # TODO (Future): This method will generate synthetic data
        # based on parameters from the UI (e.g., 'number_of_samples', 'noise_level')
        # It will create a new data_id, add to self.data_store,
        # and return the new data_id.
        
        print(f"TODO: Synthetic data generation logic goes here with params: {params}")
        
        # --- Placeholder Logic (will be replaced) ---
        X_synth = [[0],[0]] # Placeholder
        y_synth = [0, 0]    # Placeholder
        new_id = f"synth_{uuid.uuid4().hex[:6]}"
        self.data_store[new_id] = {"x": X_synth, "y": y_synth, "scalers": None}
        # -------------------------------------------
        
        return new_id
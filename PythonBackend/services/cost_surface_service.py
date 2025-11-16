def get_mock_surface_data():
    """
    Returns mock data for testing cost surface calculations.
    """
    return {
        "surface_id": "mock_surface_001",
        "cost_values": [
            [1, 2, 3],
            [4, 5, 6],
            [7, 8, 9]
        ],
        "metadata": {
            "created_by": "test_user",
            "creation_date": "2024-06-01"
        }
    }
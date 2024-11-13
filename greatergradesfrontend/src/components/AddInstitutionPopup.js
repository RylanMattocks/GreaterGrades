import { useState, useContext } from "react";
import { addInstitution } from "../greatergradesapi/Institutions";
import { UserContext } from "../functions/UserContext";
import axios from "axios";

const AddInstitutionPopup = ({ onClose }) => {
    const [error, setError] = useState('');
    const [institutionName, setInstitutionName] = useState('');
    const [address, setAddress] = useState('');
    const [city, setCity] = useState('');
    const [state, setState] = useState('');
    const [postalCode, setPostalCode] = useState('');
    const [validationError, setValidationError] = useState('');
    const { authToken } = useContext(UserContext);

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!institutionName) {
            setError('Name is required.');
            return;
        }
        if (!address || !city || !state || !postalCode) {
            setError('All address fields are required.');
            return;
        }

        try {
            const response = await axios.get(`https://api.geoapify.com/v1/geocode/search`, {
                params: {
                    text: `${address}, ${city}, ${state}, ${postalCode}`,
                    apiKey: process.env.REACT_APP_GEOAPIFY_API_KEY,
                },
            });

            if (response.data.features.length === 0) {
                setValidationError('Invalid address. Please check the details.');
                return;
            }

            const fullAddress = `${address}, ${city}, ${state} ${postalCode}`;
            const result = await addInstitution(institutionName, fullAddress, authToken);
            
            if (result) {
                onClose(); // This will trigger the refresh in parent
            } else {
                setError('Failed to add institution.');
            }
        } catch (error) {
            setValidationError('Address validation failed. Please try again.');
            console.error(error);
        }
    };

    return (
        <div className="popup-overlay">
            <div className="popup">
                <h2>Add New Institution</h2>
                <form onSubmit={handleSubmit}>
                    <div>
                        <label htmlFor="institutionName">Institution Name:</label>
                        <input
                            type="text"
                            id="institutionName"
                            value={institutionName}
                            onChange={(e) => setInstitutionName(e.target.value)}
                            required
                        />
                    </div>
                    <div>
                        <label htmlFor="address">Address:</label>
                        <input
                            type="text"
                            id="address"
                            value={address}
                            onChange={(e) => setAddress(e.target.value)}
                            required
                        />
                    </div>
                    <div>
                        <label htmlFor="city">City:</label>
                        <input
                            type="text"
                            id="city"
                            value={city}
                            onChange={(e) => setCity(e.target.value)}
                            required
                        />
                    </div>
                    <div>
                        <label htmlFor="state">State:</label>
                        <input
                            type="text"
                            id="state"
                            value={state}
                            onChange={(e) => setState(e.target.value)}
                            required
                        />
                    </div>
                    <div>
                        <label htmlFor="postalCode">Postal Code:</label>
                        <input
                            type="text"
                            id="postalCode"
                            value={postalCode}
                            onChange={(e) => setPostalCode(e.target.value)}
                            required
                        />
                    </div>
                    {error && <p className="error">{error}</p>}
                    {validationError && <p className="error">{validationError}</p>}
                    <button type="submit">Submit</button>
                    <button type="button" onClick={onClose}>Cancel</button>
                </form>
            </div>
        </div>
    );
};

export default AddInstitutionPopup;

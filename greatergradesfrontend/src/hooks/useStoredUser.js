import { useState, useEffect } from "react";
import { useLogin } from "../greatergradesapi/Auth";

const useStoredUser = (username, password) => {
    const [storedUser, setStoredUser] = useState(() => JSON.parse(localStorage.getItem('currentUser')) || null);
    const token = useLogin(username, password);

    useEffect(() => {
        if (!storedUser && token) {
            const user = JSON.parse(localStorage.getItem('currentUser'));
            setStoredUser(user);
        }
    }, [token, storedUser]);

    return storedUser;
};

export default useStoredUser;

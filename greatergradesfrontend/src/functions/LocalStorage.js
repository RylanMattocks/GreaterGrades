import {jwtDecode} from 'jwt-decode';

export function setStorageItem(key, value) {
    const now = new Date();
    const expiration = 1800 * 1000
    const item = {
        value: value,
        expire: now.getTime() + expiration
    };
    localStorage.setItem(key, JSON.stringify(item));
}

export function checkExpired(key) {
    const token = localStorage.getItem(key);
    if (!token) {
        return true;
    }

    try {
        const decodedToken = jwtDecode(token);
        const currentTime = Date.now() / 1000;
        if (decodedToken.exp < currentTime) {
            localStorage.removeItem(key);
            return true;
        }

        return false;
    } catch (error) {
        console.error("Error decoding token:", error);
        return true;
    }
}

export function getStorageItem(key) {
    const itemStr = localStorage.getItem(key);
    if (!itemStr) return null;

    const item = JSON.parse(itemStr);
    const now = new Date();

    if (now.getTime() > item.expire) {
        localStorage.removeItem(key);
        return null;
    }
    return item.value;
}
import { useEffect, useState, useContext } from "react";
import { UserContext } from '../functions/UserContext';

const url = 'http://localhost:5000/api/Classes/'
const getCommonHeader = (token) => ({
    headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
    },
});


export const useGetAllClasses = (refreshTrigger) => {
    const [classes, setClasses] = useState([]);
    const { authToken } = useContext(UserContext);

    useEffect(() => {
        const fetchClasses = async () => {
            try {
                const response = await fetch(`${url}`, getCommonHeader(authToken))
                const data = await response.json();
                setClasses(data || [])
            } catch {
                console.error("Failed to fetch classes")
            }
        }

        if (authToken) {
            fetchClasses();
        }
    }, [authToken, refreshTrigger])
    
    return classes;
}


export const addClass = async (subject, institutionId, authToken) => {
    try {
        const response = await fetch(`${url}`, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${authToken}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ subject, institutionId })
        });
        if (!response.ok) throw new Error("Failed to add class");
        return await response.json();
    } catch (error) {
        console.error("Error adding class:", error);
        return null;
    }
};


export const useGetClassById = (id) => {
    const [course, setCourse] = useState({});
    const { authToken } = useContext(UserContext);
    const [refreshTrigger, setRefreshTrigger] = useState(0);

    useEffect(() => {
        const fetchClass = async () => {
            try {
                const response = await fetch(`${url}${id}`, getCommonHeader(authToken))
                const data = await response.json();
                setCourse(data || {});
            } catch {
                console.error("Error fetching class");
            }
        }

        // Initial fetch
        if (authToken && id) {
            fetchClass();
        }
    }, [authToken, id, refreshTrigger]);

    // Function to force refresh
    const refresh = () => setRefreshTrigger(prev => prev + 1);

    return { course, refresh };
};


export const useUpdateClass = (id, subject) => {
    const { authToken } = useContext(UserContext);

    useEffect(() => {
        const fetchUpdateClass = async () => {
            try {
                const response = await fetch(`${url}${id}`, {
                    method: 'PUT',
                    headers: {
                        'Authorization': `Bearer ${authToken}`,
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ subject })
                })
                if (response.status !== 204) throw new Error();
            } catch {
                console.error("Error updating class");
            }
        }
        if (authToken && id && subject) fetchUpdateClass();
    }, [authToken, id, subject])
}


export const useDeleteClass = (id) => {
    const { authToken } = useContext(UserContext);

    useEffect(() => {
        const fetchDeleteClass = async () => {
            try {
                const response = await fetch(`${url}${id}`, {
                    method: 'DELETE',
                    headers: {
                        'Authorization': `Bearer ${authToken}`,
                        'Content-Type': 'application/json'
                    },
                })
                if (response.status !== 204) throw new Error();
            } catch {
                console.error("Error deleting class");
            }
        }
        if (authToken && id) fetchDeleteClass();
    }, [authToken, id])
}


export const addTeacherToClass = async (id, teacherId, authToken) => {
    try {
        const response = await fetch(`${url}${id}/teachers/${teacherId}`, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${authToken}`,
                'Content-Type': 'application/json'
            },
        });
        if (response.status !== 204) throw new Error("Failed to add teacher");
    } catch (error) {
        console.error("Error adding teacher to class:", error);
    }
};


export const addStudentToClass = async (id, studentId, authToken, refreshCallback) => {
    try {
        const response = await fetch(`${url}${id}/students/${studentId}`, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${authToken}`,
                'Content-Type': 'application/json'
            },
        });
        if (response.status === 204) {
            if (refreshCallback) refreshCallback();
            return 'Added';
        }
        throw new Error("Failed to add student");
    } catch (error) {
        console.error("Error adding student to class:", error);
        return null;
    }
};



export const deleteTeacherFromClass = async (classId, teacherId, authToken, refreshCallback, optimisticUpdate) => {
    try {
        // Call optimistic update immediately
        if (optimisticUpdate) {
            optimisticUpdate(classId, teacherId);
        }
        const response = await fetch(`${url}${classId}/teachers/${teacherId}`, {
            method: 'DELETE',
            headers: {
                'Authorization': `Bearer ${authToken}`,
                'Content-Type': 'application/json'
            },
        })
        if (response.status === 204) {
            if (refreshCallback) refreshCallback();
            return 'Deleted';
        }
        throw new Error('Failed to delete student');
    } catch (error) {
        console.error('Error deleting student from class:', error);
        return null;
    }
}


export const deleteStudentFromClass = async (classId, studentId, authToken, refreshCallback, optimisticUpdate) => {
    try {
        // Call optimistic update immediately
        if (optimisticUpdate) {
            optimisticUpdate(classId, studentId);
        }

        const response = await fetch(`${url}${classId}/students/${studentId}`, {
            method: 'DELETE',
            headers: {
                'Authorization': `Bearer ${authToken}`,
                'Content-Type': 'application/json'
            }
        });
        
        if (response.status === 204) {
            if (refreshCallback) refreshCallback();
            return 'Deleted';
        }
        throw new Error('Failed to delete student');
    } catch (error) {
        console.error('Error deleting student from class:', error);
        return null;
    }
};

//// Fetches not specifially tied to a single endpoint

export const useGetUsersClasses = (ids) => {
    const [classes, setClasses] = useState([]);
    const { authToken } = useContext(UserContext);
    
    useEffect(() => {
        const fetchCourses = async () => {
            try {
                const responses = await Promise.all(
                    ids.map(id => 
                        fetch(`${url}${id}`, getCommonHeader(authToken))
                    )
                )
                const data = await Promise.all(responses.map(res => {
                    if (!res.ok) throw new Error();
                    return res.json();
                }))
                setClasses(data);
            } catch {
                console.error("Error fetching classes")
            }
        }

        // Initial fetch
        if (authToken && ids?.length > 0) {
            fetchCourses();
        }

    }, [authToken, ids])

    return classes;
}
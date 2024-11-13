import { useContext } from "react";
import { UserContext } from '../functions/UserContext';
import Tiles from "./Tiles";
const EnrolledClasses = ({setSelectedItem}) => {

    const { currentUser } = useContext(UserContext);

    return (
        <div className="student-content">
            <div>
                <h3>Enrolled Classes</h3>
                <div className="tiles">
                    <Tiles courseIds={currentUser?.classIds} setSelectedItem={setSelectedItem} />
                </div>
            </div>
        </div>
    )
}

export default EnrolledClasses;
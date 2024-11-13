import { useContext } from "react";
import { UserContext } from "../functions/UserContext";
import { useGetAllClasses } from "../greatergradesapi/Classes";
import Tiles from "./Tiles";
import InstitutionTile from "./InstitutionTile";
import { useGetAllInstitutions } from "../greatergradesapi/Institutions";

const AdminContent = ({setSelectedItem}) => {
    const { currentUser } = useContext(UserContext);
    const allClasses = useGetAllClasses();
    const allInstitutions = useGetAllInstitutions();
    const classIds = allClasses.flatMap(course => course.classId);
    
    return (
        <div>
            {currentUser?.role === 2 ? 
            <div>
                <h3>Classes</h3>
                <Tiles courseIds={classIds} setSelectedItem={setSelectedItem}/>
            </div>
                :
            <div>
                <h3>Classes:</h3>
                <Tiles courseIds={classIds} setSelectedItem={setSelectedItem}/>
                <h3>Institutions:</h3>
                <div className="tiles-container">
                    {allInstitutions.map(institution => (
                        < InstitutionTile institution={institution} />
                    ))}
                </div>
            </div>
            }
        </div>
    )
}

export default AdminContent;
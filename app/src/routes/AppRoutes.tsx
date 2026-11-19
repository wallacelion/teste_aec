
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import PageLogin from '../pages/login';
import PageUserMembers from '../pages/user/members';
import PageUserMembersCreate from '../pages/user/membercreate';
import MainframeUser from '../components/layout/MainframeUser/MainframeUser';
import PageUserMembersEdit from '../pages/user/memberedit';

const AppRoutes = () => {
    return (
        <Router>
            <Routes>
                <Route path='user' element={<MainframeUser />}>
                    <Route key="Home" path='' element={<p className='p-10'>Teste CRUD AeC</p>} />
                    <Route key="Members" path='members' >
                        <Route key="home" path='' element={<PageUserMembers />} />
                        <Route key="Members" path='create' element={<PageUserMembersCreate />} />
                        <Route key="Membersedit" path=':id' element={<PageUserMembersEdit />} />
                    </Route>
                    <Route path='*' element={<>Pages not found User</>} />
                </Route>
                <Route path="*" element={<PageLogin />} />
            </Routes>
        </Router>
    );
}

export default AppRoutes;
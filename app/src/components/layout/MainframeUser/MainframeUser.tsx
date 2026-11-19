import { useState, useEffect } from 'react';

import { Outlet, useLocation, useNavigate, useParams } from 'react-router-dom';

import {
    Button,
    Menu
} from 'antd';
import type { MenuProps } from 'antd';

import { PoweroffOutlined, HomeOutlined } from '@ant-design/icons';


import useAuth from '../../../hooks/useAuth';
import useApp from '../../../hooks/useApp';

import './mainframeuser.scss';


type MenuItem = Required<MenuProps>['items'][number];

const MainframeUser = () => {

    const { setLogout } = useAuth();
    const { setBreadcrumbMenu } = useApp();

    const navigate = useNavigate();

    const [current, setCurrent] = useState('home');

    const path = useLocation();


    const items: MenuItem[] = [
        {
            key: '/user',
            label: 'Home'
        },
        {
            key: '/user/members',
            label: 'Usuários'
        },
    ];

    const siteMap = [
        {
            key: '/user',
            label: 'Home',
            title: <HomeOutlined />,
        },
        {
            key: '/user/members',
            label: 'Usuários',
        },
        {
            key: '/user/members/create',
            label: 'Criar Usuário',
        },
        {
            key: '/user/address',
            label: 'Endereços',
        },
        {
            key: '/user/configuration',
            label: 'Configuração',
        }
    ];

    const generateBreadcrumpMenu = (data: any) => {
        let menu: any = [];

        data.map((item: any, id: number) => {
            let current: any = siteMap.find((menuItem: any) => menuItem.key === `/${data.slice(0, id + 1).join('/')}`);
            if (current) {
                if (!current?.title) {
                    current.title = current.label;
                }

                current.onClick = () => navigate(current.key);

                menu.push(current);
            }
        })


        return menu;
    }


    const onHandleMenu = (e: any) => {
        setCurrent(e.key);
        navigate(e.key);
    };

    useEffect(() => {
        const listPath: string[] = path.pathname.split('/').slice(1);
        const breadcrumbMenu = generateBreadcrumpMenu(listPath);
        setBreadcrumbMenu(breadcrumbMenu);
        console.log('Paths:', breadcrumbMenu);
        console.log('Current menu item:', path);
    }, [path]);

    return (
        <div id="layout-mainframe-user" >
            <header>
                <div>
                    Teste
                </div>
                <div>
                    <Menu onClick={onHandleMenu} selectedKeys={[current]} mode="horizontal" items={items} />
                </div>
                <div>
                    <Button icon={<PoweroffOutlined />} onClick={setLogout} />
                </div>
            </header>
            <section><Outlet /></section>
            <footer>
                Footer Content
            </footer>
        </div>
    );
};

export default MainframeUser;
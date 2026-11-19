import { useCallback, useState, useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';

import { Breadcrumb, Button, Modal, Space, Dropdown, Table, Input } from 'antd';
import { AiOutlineClose, AiOutlineEye, AiOutlineForm } from "react-icons/ai";

import { DownOutlined, SmileOutlined, AudioOutlined } from '@ant-design/icons';

import useApp from '../../../hooks/useApp';
import useUser from '../../../hooks/useUser';
import useAddress from '../../../hooks/useAddress';

import { DetailUser } from '../../../components/detailUser/detailUser'

import type { MenuProps } from 'antd';
;
import "./members.scss";

const PageUserMembers = () => {

    const navigate = useNavigate()

    const { breadcrumbMenu, setIsLoading } = useApp();
    const { findAllUsers, deleteUser } = useUser();
    const { deleteAddress } = useAddress();

    const [data, setData] = useState<any[] | null>(null);
    const [pageSise, setPageSize] = useState<number>(10);
    const [pageNumber, setPageNumber] = useState<number>(1);
    const [totalRecords, setTotalRecords] = useState<number>(0);
    const [lastUpdate, setLasUpdate] = useState<number>((new Date()).getTime());

    const [showDeleteModal, setShowDeleteModal] = useState<boolean>(false);
    const [selectedUserId, setSelectedUserId] = useState<any>(null);
    const [showViewModal, setShowViewModal] = useState<boolean>(false);

    const initialFilter: any = {
        search: null,
        order: null
    }

    const [filter, setFilter] = useState<any>(initialFilter);

    const firsLoad = useRef(true);

    const columns: any[] = [
        {
            title: 'Nome',
            dataIndex: 'name',
            key: 'name',
        },
        {
            title: 'Email',
            dataIndex: 'email',
            key: 'email',
        },
        {
            title: 'Ação',
            dataIndex: 'actions',
            key: 'actions',
            width: '120px',
            render: (e: any, record: any) => {
                return <>
                    <Button color="primary" type="link" icon={<AiOutlineEye />} size="small" onClick={() => {
                        setSelectedUserId(record);
                        setShowViewModal(true);
                    }} />
                    <Button color="orange" className="mx-2" type="link" icon={<AiOutlineForm />} size="small" onClick={() => { navigate(`/user/members/${record?.uid}`) }} />
                    <Button danger icon={<AiOutlineClose />} size="small" onClick={() => {
                        handleDeleteUser(record);
                    }} />
                </>
            }
        }

    ]

    const itemsMenuOrder: any = [
        { key: '1', label: 'Nome crescente', value: { field: 'name', direction: 'asc' } },
        { key: '2', label: 'Nome decrescente', value: { field: 'name', direction: 'desc' } },
        { key: '3', label: 'Email crescente', value: { field: 'email', direction: 'asc' } },
        { key: '4', label: 'Email decrecente', value: { field: 'email', direction: 'desc' } }
    ];

    const handleDeleteUser = (user: any) => {
        setSelectedUserId(user);
        setShowDeleteModal(true);
    }

    const handdleConfirmDelete = useCallback(async () => {
        setIsLoading(true)
        console.log('Delete user with ID:', selectedUserId);
        await deleteUser(selectedUserId.uid);
        //await deleteAddress(selectedUserId.address.id);
        setShowDeleteModal(false);
        setIsLoading(false)

        getData();

    }, [selectedUserId])


    const onClearFilter = useCallback(() => {
        setFilter(initialFilter);
        //getData();
    }, [initialFilter])

    const getData = useCallback(async () => {
        firsLoad.current = false;
        setIsLoading(true)
        console.log('Get Data')

        let currentFilter: any = (filter.search?.length > 0 || filter.order) ? [] : null;

        console.log('total', filter);

        if (filter.search?.length > 0) {
            const searchParams: any = ['name', 'email'].map((item: string) => {
                return `${item}=${filter.search}`;
            })
            currentFilter = searchParams;
        } else {
            currentFilter = [];
        }

        if (filter.order) {
            const orderSelected = itemsMenuOrder[~~filter.order - 1].value;
            currentFilter.push(`OrderBy=${orderSelected.field}`);
            currentFilter.push(`OrderDirection=${orderSelected.direction}`);
        }


        if (currentFilter)
            currentFilter = `&${currentFilter.join('&')}`;


        const users: any = await findAllUsers(pageNumber, pageSise, currentFilter);
        setData(users?.data || []);
        setTotalRecords(users?.pagination?.totalItems || 0);
        setLasUpdate((new Date()).getTime());
        setIsLoading(false)
    }, [data, pageNumber, pageSise, filter])

    useEffect(() => {
        getData();
    }, [pageNumber, pageSise])

    useEffect(() => {
        console.log('Filter', filter);
        if (filter.order)
            getData();

        if (!firsLoad.current && !filter.search && !filter.order)
            getData()

    }, [filter, firsLoad])

    return (
        <div id="page-user-members">
            <div className='my-5'>
                <Breadcrumb items={[...(breadcrumbMenu || [])]} />
            </div>
            <div className='mt-10 p-4'>
                <div className="area-actions-bar">
                    <div className="grid grid-cols-[auto_350px_150px] w-full g-4">
                        <div>
                            <Input.Search allowClear placeholder='Busca...' value={filter.search} onChange={(e) => setFilter({ ...filter, search: e.target.value })} onSearch={getData} />
                        </div>
                        <div className='justify-center flex'>
                            <Dropdown className='w-full' menu={{ selectedKeys: [filter?.order], selectable: true, items: itemsMenuOrder, onClick: (e) => setFilter({ ...filter, order: e?.key }) }}>
                                <div><Button iconPlacement='end' icon={<DownOutlined />} >
                                    {filter?.order ? itemsMenuOrder[~~filter.order - 1].label : "Ordenar por..."}
                                </Button></div>
                            </Dropdown>
                            <Button className='mx-4' type="primary" onClick={() => { onClearFilter() }}>Limpar filtros</Button>
                        </div>
                        <div className='justify-end flex'>
                            <Button type="primary" onClick={() => { navigate('/user/members/create') }}>Criar Usuário</Button>
                        </div>
                    </div>
                </div>
            </div>
            <div className='mt-10 p-4'>
                <Table
                    key={lastUpdate}
                    dataSource={data}
                    columns={columns}
                    loading={data === null}
                    pagination={{
                        current: pageNumber,
                        pageSize: pageSise,
                        total: totalRecords,
                        onChange: (page, pageSize) => {
                            setPageNumber(page);
                            setPageSize(pageSize);
                            // getData();
                        }
                    }}
                />
            </div>
            <Modal
                key={`modal-${selectedUserId?.uid}`}
                title="Detalhes"
                open={showViewModal}
                onCancel={() => setShowViewModal(false)}
                footer={null}
            >
                <DetailUser key={selectedUserId?.uid} user={selectedUserId?.uid} />
            </Modal>
            <Modal
                title="Confirmação"
                open={showDeleteModal}
                onCancel={() => setShowDeleteModal(false)}
                onOk={handdleConfirmDelete}
            >
                <p>Tem certeza que deseja excluir este usuário ({selectedUserId?.name})?</p>
            </Modal>
        </div>
    );
};

export default PageUserMembers;
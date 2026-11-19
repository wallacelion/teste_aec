import { useEffect, useState, useCallback } from 'react'

import { Skeleton } from 'antd';

import useUser from '../../hooks/useUser';

export const DetailUser = ({ user }: { user: any }) => {


    const { findUser } = useUser()

    const [data, setData] = useState<any>(null);

    const getData = useCallback(async () => {

        const userData: any = await findUser(user)

        setData(userData?.data);
    }, [user])

    useEffect(() => {
        getData()
    }, [])

    return (
        <div className="detail-user">
            <div className="grid grid-cols-2">
                <div className='col-span-2'>
                    <div>
                        <strong>ID</strong>
                    </div>
                    {data?.uid ?? <Skeleton />}
                </div>
                <div>
                    <div>
                        <strong>Nome</strong>
                    </div>
                    {data?.name ?? <Skeleton />}
                </div>
                <div>
                    <div>
                        <strong>Email</strong>
                    </div>
                    {data?.email ?? <Skeleton />}
                </div>
                <div className='col-span-2'>
                    <div>
                        <strong style={{ fontSize: '1.5rem', margin: '2rem 0 1rem 0', display: 'inline-block' }}>Endereço</strong>
                    </div>
                </div>
                <div className='col-span-2'>
                    <div>
                        <strong>Logradouro</strong>
                    </div>
                    {data?.address?.logradouro ?? <Skeleton />}
                </div>
                <div className=''>
                    <div>
                        <strong>Número</strong>
                    </div>
                    {data?.address?.number ?? <Skeleton />}
                </div>
                <div className=''>
                    <div>
                        <strong>Complemento</strong>
                    </div>
                    {data?.address?.complemento ?? <Skeleton />}
                </div>
                <div className=''>
                    <div>
                        <strong>Bairro</strong>
                    </div>
                    {data?.address?.bairro ?? <Skeleton />}
                </div>
                <div className=''>
                    <div>
                        <strong>Cidade</strong>
                    </div>
                    {data?.address?.localidade ?? <Skeleton />}
                </div>
                <div className=''>
                    <div>
                        <strong>Estado</strong>
                    </div>
                    {data?.address?.uf ?? <Skeleton />}
                </div>
                <div className=''>
                    <div>
                        <strong>Cep</strong>
                    </div>
                    {data?.address?.cep ?? <Skeleton />}
                </div>
            </div>
        </div>
    )
}
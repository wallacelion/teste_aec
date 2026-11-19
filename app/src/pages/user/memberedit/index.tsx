import { useCallback, useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';

import { Breadcrumb, Button, Input } from 'antd';


import useApp from '../../../hooks/useApp';

import "./memberedit.scss";
import useAddress from '../../../hooks/useAddress';
import useUser from '../../../hooks/useUser';


const PageUserMembersEdit = () => {


    const navigate = useNavigate()
    const params: any = useParams();

    const { breadcrumbMenu, setIsLoading } = useApp();
    const { findByCep, updateAddress } = useAddress();
    const { findUser, updateUser } = useUser();

    const formDataInitial = {
        name: '',
        email: '',
        password: '',
        passwordConfirm: '',
        cep: '',
        logradouro: '',
        numero: '',
        complemento: '',
        bairro: '',
        localidade: '',
        uf: ''
    }

    const [formData, setFormData] = useState<any>(formDataInitial);
    const [userData, setUserData] = useState<any>();


    const onHandleFormSubmit = useCallback(async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        if (formData.password !== formData.passwordConfirm) {
            alert('As senhas não coincidem.');
            return;
        }

        setIsLoading(true);

        let dataUser: any = {
            name: formData.name,
            email: formData.email,
            password: formData.password
        }

        const columnsUser = ['name', 'email'];

        columnsUser.map((item: string) => {
            if (dataUser[item] === userData[item])
                delete dataUser[item];
        })

        if (dataUser.password.length === 0)
            delete dataUser.password;


        console.log('User', dataUser, Object.keys(dataUser).length);

        if (Object.keys(dataUser).length > 0)
            await updateUser(params?.id, dataUser);

        let dataAddress: any = {
            cep: formData.cep?.replace(/\D/gi, ""),
            logradouro: formData.logradouro,
            number: formData.numero,
            complemento: formData.complemento,
            bairro: formData.bairro,
            localidade: formData.localidade,
            uf: formData.uf
        }

        Object.keys(dataAddress).map((item: string) => {
            if (dataAddress[item] === userData.address[item])
                delete dataAddress[item];
        })

        console.log('Address', dataAddress, Object.keys(dataAddress).length);

        if (Object.keys(dataAddress).length > 0)
            await updateAddress(userData?.address?.id, dataAddress);

        //console.log('Form Data:', formData);
        setIsLoading(false);

    }, [formData]);

    const getCepData = useCallback(async (cep: string) => {
        if (cep.length !== 8) {
            return;
        }

        setIsLoading(true);
        const address: any = await findByCep(cep);

        let addresSend: any = address?.data || {};

        (['id', 'complemento', 'number', 'userId']).map((key: string) => {
            delete addresSend[key];
            return;
        })

        setFormData({ ...formData, ...addresSend });
        setIsLoading(false);

    }, [formData]);

    const getData = useCallback(async () => {
        setIsLoading(true);
        const data: any = await findUser(params?.id);
        const user: any = {
            name: data.data?.name,
            email: data.data?.email
        };
        const address: any = {
            cep: data.data?.address?.cep?.replace(/\D/gi, ""),
            logradouro: data.data?.address?.logradouro,
            numero: data.data?.address?.number,
            complemento: data.data?.address?.complemento,
            bairro: data.data?.address?.bairro,
            localidade: data.data?.address?.localidade,
            uf: data.data?.address?.uf
        };

        const us = { ...formDataInitial, ...user, ...address };

        setUserData(data.data);

        setFormData(us);
        setIsLoading(false);

    }, [params, formData, formDataInitial])

    useEffect(() => {
        getData()
    }, [])


    return (
        <div id="page-user-members-create">
            <div className='my-5'>
                <Breadcrumb items={[...(breadcrumbMenu || [])]} />
            </div>
            <div className='mt-10 p-4'>
                <form onSubmit={onHandleFormSubmit}>
                    <div className="grid gap-4 grid-cols-1 md:grid-cols-4 ">
                        <div className="md:col-span-4 py-2">
                            <h3><strong>Informações do usuário</strong></h3>
                        </div>
                        <div>
                            <label htmlFor="name">Nome<span className='text-[#F00] text-[2rem]'></span></label>
                            <Input value={formData?.name} onChange={(e) => setFormData({ ...formData, name: e.target.value })} required id="name" placeholder="Nome" />
                        </div>
                        <div>
                            <label htmlFor="email">E-mail<span className='text-[#F00] text-[2rem]'></span></label>
                            <Input value={formData?.email} onChange={(e) => setFormData({ ...formData, email: e.target.value })} required id="email" placeholder="E-mail" type="email" />
                        </div>
                        <div>
                            <label htmlFor="senha">Senha<span className='text-[#F00] text-[2rem]'></span></label>
                            <Input value={formData?.password} onChange={(e) => setFormData({ ...formData, password: e.target.value })} id="senha" placeholder="Senha" type="password" />
                        </div>
                        <div>
                            <label htmlFor="senhaConfirm">Confirmar senha<span className='text-[#F00] text-[2rem]'></span></label>
                            <Input value={formData?.passwordConfirm} onChange={(e) => setFormData({ ...formData, passwordConfirm: e.target.value })} id="senhaConfirm" placeholder="Confirmar senha" type="password" />
                        </div>
                        <div className="md:col-span-4 py-2">
                            <h3><strong>Endereço</strong></h3>
                        </div>
                        <div>
                            <label htmlFor="cep">CEP<span className='text-[#F00] text-[2rem]'></span></label>
                            <Input value={formData?.cep?.replace(/\D/gi, "")} onChange={(e) => {
                                getCepData(e.target.value.replace(/\D/gi, ""));
                                setFormData({ ...formData, cep: e.target.value.replace(/\D/gi, "") })
                            }} required id="cep" placeholder="CEP" maxLength={8} />
                        </div>
                        <div className='md:col-span-2'>
                            <label htmlFor="logradouro">Logradouro<span className='text-[#F00] text-[2rem]'></span></label>
                            <Input value={formData?.logradouro} disabled required id="logradouro" placeholder="Logradouro" />
                        </div>
                        <div>
                            <label htmlFor="numero">Número<span className='text-[#F00] text-[2rem]'></span></label>
                            <Input value={formData?.numero} onChange={(e) => setFormData({ ...formData, numero: e.target.value })} required id="numero" placeholder="Número" />
                        </div>
                        <div>
                            <label htmlFor="complemento">Complemento<span className='text-[#F00] text-[2rem]'></span></label>
                            <Input value={formData?.complemento} onChange={(e) => setFormData({ ...formData, complemento: e.target.value })} id="complemento" placeholder="Complemento" />
                        </div>
                        <div>
                            <label htmlFor="bairro">Bairro<span className='text-[#F00] text-[2rem]'></span></label>
                            <Input value={formData?.bairro} disabled required id="bairro" placeholder="Bairro" />
                        </div>
                        <div>
                            <label htmlFor="localidade">Cidade<span className='text-[#F00] text-[2rem]'></span></label>
                            <Input value={formData?.localidade} disabled required id="localidade" placeholder="Cidade" />
                        </div>
                        <div>
                            <label htmlFor="uf">Estado<span className='text-[#F00] text-[2rem]'></span></label>
                            <Input value={formData?.uf} disabled required id="uf" placeholder="Estado" />
                        </div>
                        <div className="col-span-4 pt-6 ">
                            <hr className='border-t border-gray-300' />
                        </div>
                        <div className="col-span-4 justify-end flex">
                            <Button danger className="mx-4" onClick={() => navigate("/user/members")}>
                                Cancelar
                            </Button>
                            <Button type="primary" htmlType="submit">
                                Salvar
                            </Button>
                        </div>
                    </div>
                </form>
            </div>
        </div>
    );
}

export default PageUserMembersEdit;
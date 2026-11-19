import { useCallback, useState } from 'react';
import { Input, Button, Alert } from 'antd';

import useLogin from '../../hooks/useLogin';
import useApp from '../../hooks/useApp';
import { useNavigate } from 'react-router-dom';

import './login.scss';


type loginData = {
    username: string;
    password: string;
}

const PageLogin = () => {

    const navigate = useNavigate()

    const { isSubmitting, errorMessage, clearError, onHandleSubmit: submitLogin } = useLogin();

    const { setIsLoading } = useApp();

    const [formData, setFormData] = useState<loginData>({
        username: 'admin@example.com',
        password: 'Senha@123'
    })

    const onHandleSubmit = useCallback(async (e: React.FormEvent<HTMLFormElement>) => {
        setIsLoading(true);
        const result = await submitLogin(e, formData);
        console.log(result);
        if (result) {
            navigate('/user');
        }
        setIsLoading(false);
    }, [formData, submitLogin, setIsLoading]);


    return (
        <div id="page-login">
            <div className="main-box">
                <form onSubmit={onHandleSubmit}>
                    <div className="grid grid-cols-1 mt-5 mb-10">
                        <div className="text-center text-2xl font-bold">Login</div>
                    </div>
                    {errorMessage && (
                        <div className="grid grid-cols-1 my-5">
                            <Alert
                                type="error"
                                message={errorMessage}
                                showIcon
                                closable
                            />
                        </div>
                    )}

                    <div className="grid grid-cols-1 my-5">
                        <label htmlFor="username">Usuário<span className='text-[#F00] text-[2rem]'>*</span></label>
                        <div>
                            <Input
                                required
                                id='username'
                                type="email"
                                value={formData?.username || ''}
                                onChange={(e) => {
                                    clearError();
                                    setFormData({ ...formData, username: e.target.value });
                                }}
                            />
                        </div>
                    </div>
                    <div className="grid grid-cols-1 my-5">
                        <label htmlFor="password">Senha<span className='text-[#F00] text-[2rem]'>*</span></label>
                        <div>
                            <Input.Password
                                required
                                id='password'
                                type="password"
                                value={formData?.password || ''}
                                onChange={(e) => {
                                    clearError();
                                    setFormData({ ...formData, password: e.target.value });
                                }}
                            />
                        </div>
                        <div className='mt-8 flex justify-center'>
                            <Button type="primary" htmlType="submit" loading={isSubmitting}>
                                Entrar
                            </Button>
                        </div>
                    </div>
                </form>
            </div>
        </div>
    );
}

export default PageLogin;
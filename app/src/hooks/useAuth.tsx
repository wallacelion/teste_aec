import { useNavigate } from "react-router-dom";
import useApp from "./useApp";

const TOKEN_REFRESH = "testaec";

const useAuth = () => {

    const navigate = useNavigate();

    const { setTokenAuth } = useApp();

    const registerToken = (data: any) => {

        setTokenAuth(data.accessToken);

        var date = new Date();
        date.setTime(date.getTime() + (30 * 24 * 60 * 60 * 1000));
        window.document.cookie = `${TOKEN_REFRESH}=${data.refreshToken.toString()};expires=${date.toUTCString()};`;
    }

    const removeToken = () => {
        window.document.cookie = `${TOKEN_REFRESH}=;expires=Thu, 01 Jan 1970 00:00:00 UTC;`;
    }

    const setLogin = (data: any) => {

        registerToken(data);
    }

    const setLogout = () => {
        removeToken();
        navigate("/");
    }

    const getRefreshToken = () => {
        const cookies = document.cookie.split(';');
        const refreshToken = cookies.find((cookie) => cookie.includes(TOKEN_REFRESH));
        return refreshToken ? refreshToken.split('=')[1] : null;
    }

    return {
        setLogin,
        setLogout,
        getRefreshToken
    }

};

export default useAuth;
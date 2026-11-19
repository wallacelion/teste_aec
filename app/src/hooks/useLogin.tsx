import { useCallback, useState } from "react";

import useApp from "./useApp";
import { loginRequest } from "../modules/auth/api/authApi";
import { ApiError } from "../shared/infrastructure/httpClient";
import useAuth from "./useAuth";

type LoginData = {
    username: string;
    password: string;
};

const useLogin = () => {
    const { setTokenAuth, setIsLoading } = useApp();
    const { setLogin } = useAuth()

    const [isSubmitting, setIsSubmitting] = useState(false);
    const [errorMessage, setErrorMessage] = useState<string | null>(null);

    const clearError = useCallback(() => {
        setErrorMessage(null);
    }, []);

    const onHandleSubmit = useCallback(async (e: React.FormEvent<HTMLFormElement>, formData: LoginData) => {
        e.preventDefault();

        setIsSubmitting(true);
        setIsLoading(true);
        setErrorMessage(null);

        try {
            const response = await loginRequest(formData);

            if (!response.accessToken) {
                throw new Error("Usuário ou senha inválidos.");
            }

            setTokenAuth(response.accessToken);
            setLogin(response)
            sessionStorage.setItem("accessToken", response.accessToken);
            //localStorage.setItem("refreshToken", response.refreshToken || "");

            return true;
        } catch (error) {
            if (error instanceof ApiError) {
                setErrorMessage(error.message);
            } else if (error instanceof Error) {
                setErrorMessage(error.message);
            } else {
                setErrorMessage("Não foi possível realizar login.");
            }

            return false;
        } finally {
            setIsSubmitting(false);
            setIsLoading(false);
        }
    }, [setIsLoading, setTokenAuth]);

    return {
        isSubmitting,
        errorMessage,
        clearError,
        onHandleSubmit,
    };
};

export default useLogin;

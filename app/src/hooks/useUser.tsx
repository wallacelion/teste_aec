import { useCallback } from "react";
import useApp from "./useApp";
import useAuth from "./useAuth";

import { findAll, createNewUser, findOne, updateDataUser, deleteDataUser } from "../modules/user/api/userApi";

const useUser = () => {
    const { setUser, tokenAuth } = useApp();
    const { getRefreshToken } = useAuth();

    const findAllUsers = useCallback(async (paseNumber?: number, pageSize?: number, filter?: any) => {

        let response = null

        try {
            const token = tokenAuth || sessionStorage.getItem("accessToken") || "";
            const payloadUser: any = {}
            payloadUser.headers = {
                "Authorization": `Bearer ${token}`,
            };
            response = await findAll(payloadUser, paseNumber, pageSize, filter)
        } catch (error) {
            console.log(error)
            //throw new Error("Não foi possível buscar os usuários.");
        }


        return response;

    }, [])

    const findUser = useCallback(async (uid: string) => {

        let response = null

        try {
            const token = tokenAuth || sessionStorage.getItem("accessToken") || "";
            const payloadUser: any = {}
            payloadUser.headers = {
                "Authorization": `Bearer ${token}`,
            };
            response = await findOne(payloadUser, uid)
        } catch (error) {
            console.log(error)
            //throw new Error("Não foi possível buscar os usuários.");
        }


        return response;

    }, [])

    const createUser = useCallback(async (userData: any) => {
        let response = null

        try {
            const token = tokenAuth || sessionStorage.getItem("accessToken") || "";
            const payloadUser: any = {}
            payloadUser.headers = {
                "Authorization": `Bearer ${token}`,
            };
            response = await createNewUser(payloadUser, userData)
        } catch (error) {
            console.log(error)
            //throw new Error("Não foi possível buscar os usuários.");
        }


        return response;
    }, [])

    const updateUser = useCallback(async (uid: string, userData: any) => {
        let response = null

        try {
            const token = tokenAuth || sessionStorage.getItem("accessToken") || "";
            const payloadUser: any = {}
            payloadUser.headers = {
                "Authorization": `Bearer ${token}`,
            };
            response = await updateDataUser(payloadUser, uid, userData)
        } catch (error) {
            console.log(error)
            //throw new Error("Não foi possível buscar os usuários.");
        }


        return response;
    }, [])

    const deleteUser = useCallback(async (uid: string) => {
        let response = null

        try {
            const token = tokenAuth || sessionStorage.getItem("accessToken") || "";
            const payloadUser: any = {}
            payloadUser.headers = {
                "Authorization": `Bearer ${token}`,
            };
            response = await deleteDataUser(payloadUser, uid)
        } catch (error) {
            console.log(error)
            //throw new Error("Não foi possível buscar os usuários.");
        }


        return response;
    }, [])

    return {
        findAllUsers,
        findUser,
        createUser,
        updateUser,
        deleteUser
    }
};

export default useUser;
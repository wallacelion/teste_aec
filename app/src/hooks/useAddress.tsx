import { useCallback } from "react";
import useApp from "./useApp";
import useAuth from "./useAuth";

import { findCep, createNewAddress, updateDataAddress, deleteDataAddress } from "../modules/address/api/addressApi";

const useAddress = () => {
    const { setUser, tokenAuth } = useApp();
    const { getRefreshToken } = useAuth();

    const findByCep = useCallback(async (cep: string) => {

        let response = null

        try {
            const token = tokenAuth || sessionStorage.getItem("accessToken") || "";
            const payloadUser: any = {}
            payloadUser.headers = {
                "Authorization": `Bearer ${token}`,
            };
            response = await findCep(payloadUser, cep)
        } catch (error) {
            console.log(error)
            //throw new Error("Não foi possível buscar os usuários.");
        }


        return response;

    }, [])

    const createAddress = useCallback(async (userData: any) => {
        let response = null

        try {
            const token = tokenAuth || sessionStorage.getItem("accessToken") || "";
            const payloadUser: any = {}
            payloadUser.headers = {
                "Authorization": `Bearer ${token}`,
            };
            response = await createNewAddress(payloadUser, userData)
        } catch (error) {
            console.log(error)
            //throw new Error("Não foi possível buscar os usuários.");
        }


        return response;
    }, [])

    const updateAddress = useCallback(async (uid: string, userData: any) => {
        let response = null

        try {
            const token = tokenAuth || sessionStorage.getItem("accessToken") || "";
            const payloadUser: any = {}
            payloadUser.headers = {
                "Authorization": `Bearer ${token}`,
            };
            response = await updateDataAddress(payloadUser, uid, userData)
        } catch (error) {
            console.log(error)
            //throw new Error("Não foi possível buscar os usuários.");
        }


        return response;
    }, [])

    const deleteAddress = useCallback(async (uid: string) => {
        let response = null

        try {
            const token = tokenAuth || sessionStorage.getItem("accessToken") || "";
            const payloadUser: any = {}
            payloadUser.headers = {
                "Authorization": `Bearer ${token}`,
            };
            response = await deleteDataAddress(payloadUser, uid)
        } catch (error) {
            console.log(error)
            //throw new Error("Não foi possível buscar os usuários.");
        }


        return response;
    }, [])

    return {
        findByCep,
        createAddress,
        updateAddress,
        deleteAddress
    }
};

export default useAddress;
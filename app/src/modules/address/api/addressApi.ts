import { postJson, getJson, pathJson, deleteJson } from "../../../shared/infrastructure/httpClient";

interface ResponseRaw {
    statusCode?: string;
    status?: string;
    data?: any;
}


export const findCep = async (payload: any, cep: string): Promise<ResponseRaw | []> => {

    const raw = await getJson<ResponseRaw>(`/api/address/search/${cep}`, payload);
    return raw || [];
}

export const createNewAddress = async (payloadUser: any, userData: any): Promise<ResponseRaw | null> => {
    const raw = await postJson<ResponseRaw, any>(`/api/address`, userData, payloadUser);
    return raw || [];
}

export const deleteDataAddress = async (payloadUser: any, id: any): Promise<ResponseRaw | null> => {
    const raw = await deleteJson<ResponseRaw>(`/api/address/${id}`, payloadUser);
    return raw;
}

export const updateDataAddress = async (payloadUser: any, id: any, userData: any): Promise<ResponseRaw | null> => {
    const raw = await pathJson<ResponseRaw, any>(`/api/address/${id}`, userData, payloadUser);
    return raw;
}
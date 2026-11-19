import { postJson, getJson, pathJson, deleteJson } from "../../../shared/infrastructure/httpClient";

interface ResponseRaw {
    statusCode?: string;
    status?: string;
    data?: any;
}

export interface ListUsers {
    uid: string;
    name: string;
    email: string;
}

export interface newUser {
    name: string;
    email: string;
    password: string;
}

export const findAll = async (payloadUser: any, pageNumber: number = 1, pageSize: number = 10, filter: any = null): Promise<ResponseRaw | []> => {

    const raw = await getJson<ResponseRaw>(`/api/user?pageNumber=${pageNumber}&pageSize=${pageSize}${filter || ''}`, payloadUser);
    return raw || [];
}

export const findOne = async (payloadUser: any, uid: any): Promise<ResponseRaw | []> => {

    const raw = await getJson<ResponseRaw>(`/api/user/${uid}`, payloadUser);
    return raw || [];
}

export const createNewUser = async (payloadUser: any, userData: any): Promise<ResponseRaw | null> => {
    const raw = await postJson<ResponseRaw, any>(`/api/user`, userData, payloadUser);
    return raw || [];
}

export const deleteDataUser = async (payloadUser: any, uid: any): Promise<ResponseRaw | null> => {
    const raw = await deleteJson<ResponseRaw>(`/api/user/${uid}`, payloadUser);
    return raw;
}

export const updateDataUser = async (payloadUser: any, uid: any, userData: any): Promise<ResponseRaw | null> => {
    const raw = await pathJson<ResponseRaw, any>(`/api/user/${uid}`, userData, payloadUser);
    return raw;
}
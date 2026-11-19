import { useLayoutEffect } from "react";

import AppRoutes from "../../routes/AppRoutes";
import useApp from "../../hooks/useApp";


const NavigationBase = () => {

    const { setIsLoading } = useApp();

    useLayoutEffect(() => {
        setIsLoading(false);
    }, [])

    return (
        <>
            <AppRoutes />
        </>
    );
}

export default NavigationBase;
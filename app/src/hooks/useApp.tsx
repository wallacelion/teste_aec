import { useContext } from "react";
import { AppContent } from "../providers/App";

const useApp = () => {
    const appContext = useContext(AppContent);

    if (!appContext) {
        throw new Error("useApp must be used within an AppProvider");
    }

    return appContext;
}

export default useApp;
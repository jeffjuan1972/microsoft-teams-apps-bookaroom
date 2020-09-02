/*
    <copyright file="router.tsx" company="Microsoft Corporation">
    Copyright (c) Microsoft Corporation. All rights reserved.
    </copyright>
*/

import React from 'react';
import { Route } from "react-router-dom";
import AddFavorites from "../components/add-favorites";
import OtherRoom from "../components/other-room";
import OtherRoomNow from "../components/other-room-now";

const ReactRouter = () => {
    return (
        <React.Fragment>
            <Route path="/Meeting/OtherRoom/" component={OtherRoom} />
            <Route path="/Meeting/OtherRoomNow/" component={OtherRoomNow} />
            <Route path="/Meeting/AddFavourite/" component={AddFavorites} />
        </React.Fragment>
    );
}
export default ReactRouter;

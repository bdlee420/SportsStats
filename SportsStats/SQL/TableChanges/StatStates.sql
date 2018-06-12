alter table statstates add GameID int 

UPDATE SS SET SS.GameID = (select gameID FROM Stats WHERE ID = SS.StatID) FROM StatStates SS 
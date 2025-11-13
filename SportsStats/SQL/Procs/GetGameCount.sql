CREATE PROCEDURE [dbo].[GetGameCount]	
    @TeamID		int,
	@LeagueID	int
AS
BEGIN
	select count(*) as GameCount
	from 
		(	select	distinct s.gameID
			from	stats s
			join	games g on g.id = s.GameID
			join	leagues l on l.id = g.LeagueID
			where	l.id = @LeagueID and s.TeamID = @TeamID
		) x
END
function messageBox(messageBoxText) {
    if (confirm(messageBoxText) === true) {
        return true;
    }
    else {
        return false;
    }
}

function FilterStats(playerStats, statType, x, numberFormat) {
    var stat = playerStats.filter(function (el) {
        return el.Name === statType;
    });

    if (stat === undefined || stat.length === 0) {
        return null;
    }

    stat[0].Filter = numberFormat;
    return stat[0];
}

function UpdateStatsObject(statTypes, playerStats, teamID, gameID, leagueID) {
    for (var n = 0; n < playerStats.length; n++) {
        var stats = playerStats[n];
        for (var x = 0; x < statTypes.length; x++) {            
            var currentStatType = statTypes[x];
            var numberFormat = '';
            if (currentStatType.ValueType === 2) {
                numberFormat = 'percentage:1';
            }
            else if (currentStatType.ValueType === 3) {
                numberFormat = 'decimal:3';
            }
            else if (currentStatType.ValueType === 4) {
                numberFormat = 'decimal:1';
            }
            var statType2 = currentStatType.Name;
            var statValue2 = new FilterStats(stats.PlayerStats, statType2, x, numberFormat, leagueID);
            if (statValue2 != null) {
                stats[statType2] = statValue2.Value;
                stats.GameID = gameID;
                stats.TeamID = teamID;
            }
        }
    }
}

(function ($) {
    $(document).ready(function () {
        $('ul.dropdown-menu [data-toggle=dropdown]').on('click', function (event) {
            event.preventDefault();
            event.stopPropagation();
            $(this).parent().siblings().removeClass('open');
            $(this).parent().toggleClass('open');
        });
    });
})(jQuery);
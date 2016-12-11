
function startAJAX() {
    descartesRequire("../service/AprioriService.asmx/SearchPaperData", "200");
}

function controlTabCurrent() {

    var element = $(this);

    element.removeClass('current');
    element.addClass('current');

    //element.toggleClass('current');

    $('aside > section:nth-child(1) ~ section').css('display', 'none');

    $('aside > section:nth-child(1) ~ section').each(function (index) {
        if ($(this).prop('id') == element.prop('id').replace("-control", "")) {
            $(this).css('display', 'block');
        }
    });

    $('ul.tabControl li').each(function (index) {
        if (index != $("ul.tabControl li").index(element)) {
            $(this).removeClass('current');
        }
    });
}

function controlListCurrent() {

    var element = $(this);
    element.toggleClass('current');

    if (element.parent().parent().prop('id') == 'paper-window') {
        $('aside > section#paper-window > ul li').each(function (index) {
            if (index != $("aside > section#paper-window > ul li").index(element)) {
                $(this).removeClass('current');
            }
        });
    }
    else if (element.parent().parent().prop('id') == 'list-window') {
        $('section#list-window > ul li').each(function (index) {
            if (index != $("section#list-window > ul li").index(element)) {
                $(this).removeClass('current');
            }
        });
    }

}

function startSearching() {

    var inputValue = $("#searching-window > section input").val();

    if (!(inputValue === '' || inputValue === null)) descartesRequire("../service/AprioriService.asmx/SearchPaperData", inputValue)
    else alert("请正确输入搜索关键字");

    $('section#list-window').css('display', 'none');
}

function descartesRequire(url, data) {

    var options = {
        type: "POST",
        cache: false,
        url: url,
        data: { Data: data },
        //contentType: "application/json; charset=utf-8",
        //dataType: "json",
        success: function (response) {

            //$('main > textarea')[0].innerText = $(response).find('string').text();

            var descartesData = $(response).find('string').text()

            //alert(descartesData);

            descartesData = JSON.parse(descartesData);

            loadDescartes(descartesData);
        },
        error: function (jqXHR, textStatus, errorThrown) {

            alert("网络不通畅，请稍后再试");
        }
    };
    $.ajax(options);
}

function loadDescartes(descartesData) {

    var BasicData;
    var ConcatData;
    var geoCoordMap;

    var centrePaperId = 0;

    var bmap = {
        center: [107, 39],
        zoom: 5,
        roam: true,
        mapStyle: {
            styleJson: [{
                'featureType': 'water',
                'elementType': 'all',
                'stylers': {
                    'color': '#B4DCED'
                }
            }, {
                'featureType': 'land',
                'elementType': 'all',
                'stylers': {
                    'color': '#F5F5F5'
                }
            }, {
                'featureType': 'railway',
                'elementType': 'all',
                'stylers': {
                    'visibility': 'off'
                }
            }, {
                'featureType': 'highway',
                'elementType': 'all',
                'stylers': {
                    'visibility': 'off'
                }
            }, {
                'featureType': 'highway',
                'elementType': 'labels',
                'stylers': {
                    'visibility': 'off'
                }
            }, {
                'featureType': 'arterial',
                'elementType': 'geometry',
                'stylers': {
                    'visibility': 'off'
                }
            }, {
                'featureType': 'arterial',
                'elementType': 'geometry.fill',
                'stylers': {
                    'visibility': 'off'
                }
            }, {
                'featureType': 'arterial',
                'elementType': 'labels',
                'stylers': {
                    'visibility': 'off'
                }
            }, {
                'featureType': 'poi',
                'elementType': 'all',
                'stylers': {
                    'visibility': 'off'
                }
            }, {
                'featureType': 'green',
                'elementType': 'all',
                'stylers': {
                    'visibility': 'off'
                }
            }, {
                'featureType': 'subway',
                'elementType': 'all',
                'stylers': {
                    'visibility': 'off'
                }
            }, {
                'featureType': 'manmade',
                'elementType': 'all',
                'stylers': {
                    'visibility': 'off'
                }
            }, {
                'featureType': 'local',
                'elementType': 'all',
                'stylers': {
                    'visibility': 'off'
                }
            }, {
                'featureType': 'boundary',
                'elementType': 'all',
                'stylers': {
                    'color': '#CCCCCC'
                }
            }, {
                'featureType': 'building',
                'elementType': 'all',
                'stylers': {
                    'visibility': 'off'
                }
            }, {
                'featureType': 'label',
                'elementType': 'labels.text.fill',
                'stylers': {
                    'color': '#777777'
                }
            }]
        }
    };

    var AmountBasic;

    prepareDescartes();

    makeSeriesBasic();

    var convertData = function (data) {
        var res = [];
        for (var i = 0; i < data.length; i++) {

            var dataItem = data[i];

            var ID_1;
            var ID_2;
            for (var js in BasicData) {

                if (dataItem[0].name == BasicData[js].Subject) {
                    ID_1 = BasicData[js].ID;
                }
                if (dataItem[1].name == BasicData[js].Subject) {
                    ID_2 = BasicData[js].ID;
                }
            }

            var fromCoord = geoCoordMap[ID_1].Coords;
            var toCoord = geoCoordMap[ID_2].Coords;
            if (fromCoord && toCoord) {

                //alert(dataItem[0].name);
                //alert(dataItem[1].name);
                //alert([fromCoord, toCoord]);

                res.push({
                    fromName: dataItem[0].name,
                    toName: dataItem[1].name,
                    rate: dataItem[1].value,
                    coords: [fromCoord, toCoord]
                });
            }
        }
        return res;
    };

    function prepareDescartes() {

        var index = 0;

        for (var js in descartesData) {
            index++;

            if (index == 1) {
                BasicData = descartesData[js];
            }
            else {
                ConcatData = descartesData[js];
            }
        }

        AmountBasic = 0;

        for (var js in BasicData) {
            AmountBasic++;
        }

        index = 0;

        $('aside > section#paper-window > ul li').remove();

        geoCoordMap = "{";

        for (var js in BasicData) {

            index++;

            geoCoordMap += '"' + BasicData[js].ID + '": {"ID":' + BasicData[js].ID + ',' + '"Subject": "' + BasicData[js].Subject + '",'
                + '"Coords": [' + BasicData[js].Coords.longitude + ',' + BasicData[js].Coords.latitude + '], "Location": "'
                + BasicData[js].Location + '", "Organization": "' + BasicData[js].Organization + '","KeyWords": "' + BasicData[js].KeyWords
                + '","Author": "' + BasicData[js].Author + '","ConcatPaper": [';

            var ID = BasicData[js].ID;
            for (var i = 0; i < eval("ConcatData[" + ID + "]" + ".ConcatPaper.length"); i++) {

                geoCoordMap += '{"ID": "' + eval("ConcatData[" + ID + "]" + ".ConcatPaper[" + i + "].ID") + '","Rate":' + eval("ConcatData[" + ID + "]" + ".ConcatPaper[" + i + "].Rate");

                if (i != eval("ConcatData[" + ID + "]" + ".ConcatPaper.length") - 1) geoCoordMap += '},';
                else geoCoordMap += '}';
            }

            if (index == AmountBasic) geoCoordMap += ']}';
            else geoCoordMap += ']},';

            $('aside > section#paper-window > ul').append("<li><abbr>" + BasicData[js].Subject + "</abbr></li>");
        }

        geoCoordMap += '}';

        //alert(geoCoordMap);

        geoCoordMap = JSON.parse(geoCoordMap);

        $('aside > section#paper-window > ul li').click(controlListCurrent);
        $('aside > section#paper-window > ul li').click(function () {

            var paperIndex = 0;
            for (var js in BasicData) {

                paperIndex++;

                if ($(this).children("abbr")[0].innerText == BasicData[js].Subject) {
                    centrePaperId = paperIndex;
                    break;
                }
            }

            makeSeriesConcat($(this).children("abbr")[0].innerText);
        });
    }

    function makeSeriesConcat(Subject) {

        var seriesDataConcat;

        var ID;

        for (var js in BasicData) {

            if (Subject == BasicData[js].Subject) {
                ID = BasicData[js].ID;
                break;
            }
        }

        $('section#list-window > ul li').remove();

        seriesDataConcat = '[';

        for (var i = 0; i < eval("geoCoordMap[" + ID + "]" + ".ConcatPaper.length"); i++) {

            var ConcatID = eval("geoCoordMap[" + ID + "]" + ".ConcatPaper[" + i + "].ID");

            var ConcatSubject = eval("BasicData[" + ConcatID + "].Subject");

            seriesDataConcat += '[{"name": "' + Subject + '"}, {"name": "' + ConcatSubject + '","value": ' + eval("geoCoordMap[" + ID + "]" + ".ConcatPaper[" + i + "].Rate");

            if (i != eval("geoCoordMap[" + ID + "]" + ".ConcatPaper.length") - 1) seriesDataConcat += '}],';
            else seriesDataConcat += '}]';

            $('section#list-window > ul').append("<li><abbr>" + ConcatSubject + "</abbr></li>");
        }

        //$('section#list-window > ul li').click(controlListCurrent);

        seriesDataConcat += ']';

        //alert(seriesDataConcat);

        seriesDataConcat = JSON.parse(seriesDataConcat);

        bulidDescartesConcat(seriesDataConcat);

        $('section#list-window > p:nth-of-type(2) > a')[0].innerText = Subject;
        $('section#list-window > p:nth-of-type(3)')[0].innerText = "关联文献（共计 " + seriesDataConcat.length + " 篇）：";
        $('section#list-window').css('display', 'block');

    }

    function makeSeriesBasic() {

        var seriesDataBasic = '[';

        var index = 0;

        for (var js in BasicData) {

            index++;

            seriesDataBasic += '[{"name":"' + BasicData[js].Subject;

            if (index != AmountBasic) seriesDataBasic += '"}],';
            else seriesDataBasic += '"}]';
        }

        seriesDataBasic += ']';

        seriesDataBasic = JSON.parse(seriesDataBasic);

        bulidDescartesBasic(seriesDataBasic);

        if ($('.tabControl li#paper-window-control').hasClass('current')) {
            $('aside > section#paper-window').css('display', 'block');
        }
    }

    function bulidDescartesBasic(seriesDataBasic) {

        var myChart = echarts.init($("section.wrapper-baseMap > section.descartesResult > div.descartes")[0]);

        var optionBasic = null;

        var seriesBasic = [];

        [seriesDataBasic].forEach(function (item, i) {
            seriesBasic.push({
                name: 'Basic',
                type: 'scatter',
                coordinateSystem: 'bmap',
                //coordinateSystem: 'geo',
                zlevel: 2,
                symbolSize: 24,
                itemStyle: {
                    normal: {
                        color: '#FF6F69',
                        opacity: 0.6,
                        borderColor: '#999',
                        borderWidth: 2
                    }
                },
                data: item.map(function (dataItem, index) {

                    var ID;
                    var subject;
                    var author;
                    var location;
                    var organization;
                    var keyWords;

                    for (var js in BasicData) {

                        if (dataItem[0].name == BasicData[js].Subject) {

                            ID = BasicData[js].ID;
                            author = BasicData[js].Author;
                            location = BasicData[js].Location;
                            organization = BasicData[js].Organization;
                            keyWords = BasicData[js].KeyWords;
                            break;
                        }
                    }

                    return {
                        subject: dataItem[0].name,
                        author: '作&nbsp;&nbsp;&nbsp;者:&nbsp;&nbsp;' + author,
                        location: '位&nbsp;&nbsp;&nbsp;置:&nbsp;&nbsp;' + location,
                        organization: '单&nbsp;&nbsp;&nbsp;位:&nbsp;&nbsp;' + organization,
                        keyWords: '关键词:&nbsp;&nbsp;' + keyWords,
                        value: geoCoordMap[ID].Coords
                    };
                })
            });
        });

        optionBasic = {
            backgroundColor: 'whitesmoke',
            tooltip: {
                trigger: 'item',
                padding: 18,
                //enterable: true,
                backgroundColor: 'whitesmoke',
                textStyle: {
                    color: 'lightgray',
                    fontFamily: 'PingFang SC',
                    fontSize: '0.8em',
                },
                extraCssText: 'box-shadow: 0 4px 16px -4px #999;',
                formatter: function (params) {
                    return '<p style="color: #555;font-family: Fzqkbys;font-size: 1.2em;border-bottom: 2px solid #999;">' + params.data.subject + '</p>'
                        + '<p style="color: #555;font-family: PingFang SC;font-size: 1em;border-bottom: 1px dotted #999;">' + params.data.keyWords + '</p>'
                        + '<p style="color: #555;font-family: PingFang SC;font-size: 1em;border-bottom: 1px dotted #999;">' + params.data.author + '</p>'
                        + '<p style="color: #555;font-family: PingFang SC;font-size: 1em;border-bottom: 1px dotted #999;">' + params.data.organization + '</p>'
                        + '<p style="color: #555;font-family: PingFang SC;font-size: 1em;border-bottom: 1px dotted #999;">' + params.data.location + '</p>'
                }
            },
            bmap: bmap,
            //geo: {
            //    map: 'china',
            //    roam: true,
            //    itemStyle: {
            //        normal: {
            //            areaColor: 'whitesmoke',
            //            borderColor: '#777'
            //        },
            //        emphasis: {
            //            areaColor: '#whitesmoke'
            //        }
            //    }
            //},
            series: seriesBasic
        };

        if (optionBasic && typeof optionBasic === "object") {

            myChart.setOption(optionBasic, false, false);
            myChart.on('click', function (params) {
                if (params.seriesName == 'Basic') {

                    var paperIndex = 0;
                    for (var js in BasicData) {

                        paperIndex++;

                        if (params.data.subject == BasicData[js].Subject) {
                            centrePaperId = paperIndex;
                            break;
                        }
                    }

                    $('aside > section#paper-window > ul li').each(function (index) {
                        if (index != centrePaperId - 1) $(this).removeClass('current');
                        else { $(this).removeClass('current'); $(this).addClass('current'); }
                    });

                    makeSeriesConcat(params.data.subject);
                }
            });

            //$('aside > section#paper-window > ul li').mousemove(function () {

            //    var element = $(this);
            //    var index = $("aside > section#paper-window > ul li").index(element);

            //    myChart.dispatchAction({
            //        type: 'showTip',
            //        seriesIndex: 1,
            //        dataIndex: index
            //    });
            //});
        }
        else {
            alert('加载失败');
        }
    }

    function bulidDescartesConcat(seriesDataConcat) {

        var myChart = echarts.init($("section.wrapper-baseMap > section.descartesResult > div.descartes")[0]);

        var optionConcat = null;

        var seriesConcat = [];

        var tooltipConcatLine = {
            trigger: 'item',
            padding: 18,
            //enterable: true,
            backgroundColor: 'whitesmoke',
            textStyle: {
                color: 'lightgray',
                fontFamily: 'PingFang SC',
                fontSize: '0.8em',
            },
            extraCssText: 'box-shadow: 0 4px 16px -4px #999;',
            formatter: function (params) {
                return '<p style="color: #555;font-family: Fzqkbys;font-size: 1em;border-top: 2px solid #999; text-align: center;">' + params.data.fromName + '</p>'
                    + '<p style="color: #555;font-family: Fzqkbys;font-size: 1.2em; text-align: center;">' + '<<----->>' + '</p>'
                    + '<p style="color: #555;font-family: Fzqkbys;font-size: 1em; text-align: center;">' + params.data.toName + '</p>'
                    + '<p style="color: #555;font-family: PingFang SC;font-size: 1.2em;border-top: 2px solid #999; text-align: center;">关联度:&nbsp;&nbsp;' + params.data.rate + '</p>'
            }
        };

        var tooltipConcatPoint = {
            trigger: 'item',
            padding: 18,
            //enterable: true,
            backgroundColor: 'whitesmoke',
            textStyle: {
                color: 'lightgray',
                fontFamily: 'PingFang SC',
                fontSize: '0.8em',
            },
            extraCssText: 'box-shadow: 0 4px 16px -4px #999;',
            formatter: function (params) {
                return '<p style="color: #555;font-family: Fzqkbys;font-size: 1.2em;border-bottom: 2px solid #999;">' + params.data.subject + '</p>'
                    + '<p style="color: #555;font-family: PingFang SC;font-size: 1em;border-bottom: 1px dotted #999;">' + params.data.keyWords + '</p>'
                    + '<p style="color: #555;font-family: PingFang SC;font-size: 1em;border-bottom: 1px dotted #999;">' + params.data.author + '</p>'
                    + '<p style="color: #555;font-family: PingFang SC;font-size: 1em;border-bottom: 1px dotted #999;">' + params.data.organization + '</p>'
                    + '<p style="color: #555;font-family: PingFang SC;font-size: 1em;border-bottom: 1px dotted #999;">' + params.data.location + '</p>'
                    + '<p style="color: #555;font-family: PingFang SC;font-size: 1em;border-bottom: 1px dotted #999;">' + params.data.rate + '</p>'
                //+ '<p style="color: #555;font-family: PingFang SC;font-size: 1em;border-bottom: 1px dotted #999;">' + params.data.index + '</p>'
            }
        };

        var tooltipConcatCenter = {
            trigger: 'item',
            padding: 18,
            //enterable: true,
            backgroundColor: 'whitesmoke',
            textStyle: {
                color: 'lightgray',
                fontFamily: 'PingFang SC',
                fontSize: '0.8em',
            },
            extraCssText: 'box-shadow: 0 4px 16px -4px #999;',
            formatter: function (params) {

                return '<p style="color: #555;font-family: Fzqkbys;font-size: 1.2em;border-bottom: 2px solid #999;">' + params.data.subject + '</p>'
                    + '<p style="color: #555;font-family: PingFang SC;font-size: 1em;border-bottom: 1px dotted #999;">' + params.data.keyWords + '</p>'
                    + '<p style="color: #555;font-family: PingFang SC;font-size: 1em;border-bottom: 1px dotted #999;">' + params.data.author + '</p>'
                    + '<p style="color: #555;font-family: PingFang SC;font-size: 1em;border-bottom: 1px dotted #999;">' + params.data.organization + '</p>'
                    + '<p style="color: #555;font-family: PingFang SC;font-size: 1em;border-bottom: 1px dotted #999;">' + params.data.location + '</p>'
                //+ '<p style="color: #555;font-family: PingFang SC;font-size: 1em;border-bottom: 1px dotted #999;">' + params.data.index + '</p>'
            }
        };



        [seriesDataConcat].forEach(function (item, i) {
            seriesConcat.push({
                name: 'Concat',
                type: 'lines',
                coordinateSystem: 'bmap',
                //coordinateSystem: 'geo',
                zlevel: 1,
                lineStyle: {
                    normal: {
                        color: '#CCCCCC',
                        width: 6,
                        opacity: 0.6,
                        curveness: 0.2,
                    }
                },
                data: convertData(item)
            }, {
                    name: 'Concat',
                    type: 'scatter',
                    coordinateSystem: 'bmap',
                    //coordinateSystem: 'geo',
                    zlevel: 2,
                    symbolSize: function (val) {
                        return val[2] * 3;
                    },
                    itemStyle: {
                        normal: {
                            color: '#FF6F69',
                            opacity: 0.6,
                            borderColor: '#999',
                            borderWidth: 2
                        },
                        //emphasis: {
                        //borderColor: '#999',
                        //borderWidth: 2
                        //}
                    },
                    data: item.map(function (dataItem, index) {

                        var ID;
                        var subject;
                        var author;
                        var location;
                        var organization;
                        var keyWords;
                        var rate;

                        for (var js in BasicData) {

                            if (dataItem[1].name == BasicData[js].Subject) {

                                ID = BasicData[js].ID;
                                author = BasicData[js].Author;
                                location = BasicData[js].Location;
                                organization = BasicData[js].Organization;
                                keyWords = BasicData[js].KeyWords;

                                break;
                            }
                        }

                        return {
                            index: index,
                            subject: dataItem[1].name,
                            author: '作&nbsp;&nbsp;&nbsp;者:&nbsp;&nbsp;' + author,
                            location: '位&nbsp;&nbsp;&nbsp;置:&nbsp;&nbsp;' + location,
                            organization: '单&nbsp;&nbsp;&nbsp;位:&nbsp;&nbsp;' + organization,
                            keyWords: '关键词:&nbsp;&nbsp;' + keyWords,
                            rate: '关联度:&nbsp;&nbsp;' + dataItem[1].value,
                            value: geoCoordMap[ID].Coords.concat([dataItem[1].value] * 10)
                        };
                    })
                }, {
                    name: 'ConcatCenter',
                    type: 'scatter',
                    coordinateSystem: 'bmap',
                    //coordinateSystem: 'geo',
                    zlevel: 3,
                    symbolSize: 30,
                    itemStyle: {
                        normal: {
                            color: '#67B8DE',
                            opacity: 0.8,
                            borderColor: '#999',
                            borderWidth: 2
                        }
                    },
                    data: [{
                        name: "大",
                        subject: geoCoordMap[centrePaperId].Subject,
                        author: '作&nbsp;&nbsp;&nbsp;者:&nbsp;&nbsp;' + geoCoordMap[centrePaperId].Author,
                        location: '位&nbsp;&nbsp;&nbsp;置:&nbsp;&nbsp;' + geoCoordMap[centrePaperId].Location,
                        organization: '单&nbsp;&nbsp;&nbsp;位:&nbsp;&nbsp;' + geoCoordMap[centrePaperId].Organization,
                        keyWords: '关键词:&nbsp;&nbsp;' + geoCoordMap[centrePaperId].KeyWords,
                        value: geoCoordMap[centrePaperId].Coords
                    }]
                });
        });

        optionConcat = {
            backgroundColor: 'whitesmoke',
            tooltip: {},
            bmap: bmap,
            //visualMap: {
            //    min: 6,
            //    max: 10,
            //    calculable: true,
            //    inRange: {
            //        color: ['#50a3ba', '#eac736', '#d94e5d']
            //    },
            //    textStyle: {
            //        color: '#777'
            //    }
            //},
            //geo: {
            //    map: 'china',
            //    roam: true,
            //    itemStyle: {
            //        normal: {
            //            areaColor: 'whitesmoke',
            //            borderColor: '#777'
            //        },
            //        emphasis: {
            //            areaColor: '#whitesmoke'
            //        }
            //    }
            //},
            series: seriesConcat
        };

        if (optionConcat && typeof optionConcat === "object") {

            myChart.setOption(optionConcat, false, false);

            myChart.on('mousemove', function (params) {
                if (params.seriesName == 'Concat') {
                    if (params.seriesType == 'lines') {
                        myChart.setOption({
                            tooltip: tooltipConcatLine
                        });
                    }
                    else if (params.seriesType == 'scatter') {
                        myChart.setOption({
                            tooltip: tooltipConcatPoint
                        });
                    }
                }
                else if (params.seriesName == 'ConcatCenter') {
                    myChart.setOption({
                        tooltip: tooltipConcatCenter
                    });
                }
            });

            myChart.on('click', function (params) {
                if (params.seriesType == 'scatter') {

                    var paperIndex = 0;
                    for (var js in BasicData) {

                        paperIndex++;

                        if (params.data.subject == BasicData[js].Subject) {
                            centrePaperId = paperIndex;
                            break;
                        }
                    }

                    $('aside > section#paper-window > ul li').each(function (index) {
                        if (index != centrePaperId - 1) $(this).removeClass('current');
                        else { $(this).removeClass('current'); $(this).addClass('current'); }
                    });

                    makeSeriesConcat(params.data.subject);
                }
            });

            $('section#list-window > ul li').mouseenter(function () {

                var element = $(this);
                var index = $("section#list-window > ul li").index(element);

                myChart.setOption({
                    tooltip: tooltipConcatPoint
                });

                myChart.dispatchAction({
                    type: 'showTip',
                    seriesIndex: 1,
                    dataIndex: index
                });
            });
        }
        else {
            alert('加载失败');
        }
    }
}
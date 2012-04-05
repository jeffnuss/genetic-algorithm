	//Loading the chart API
	google.load("visualization", "1", {packages:["corechart"]});

	//Creates the chart		
   	google.setOnLoadCallback(drawChart);

	$(document).ready(function(e) {
		for(var i in genomes)
			initialize(40,-111,genomes[i],i);

	});
	
	

function drawChart() {
        var data = new google.visualization.DataTable();
        data.addColumn('number', 'Generations');
        data.addColumn('number', 'Fitness');
        data.addRows(chartdata);

        var options = {
          title: 'Algorithm Convergence',
		  hAxis: {
			title: 'Number of Iterations'},
		  vAxis: {
			title: 'Fitness'}
        };

        var chart = new google.visualization.LineChart(document.getElementById('chart_div'));
        chart.draw(data, options);
    }
	
	//Initializing the google map
	function initialize(lat,lon,genome,iter){
		
		//Creating the map
		var myOptions = {			
			center: google.maps.LatLng(lat,lon),scrollwheel: false,zoom: 11,	//Zoom Level 19 is the max zoom
			mapTypeId: google.maps.MapTypeId.HYBRID,
		}
		
		//Creating the map
		$("<h2>Genome " + iter + "</h2>").appendTo('#maps');;
		var newdiv = $("<div class='map'>").appendTo('#maps');
		var mymap = new google.maps.Map(newdiv[0], myOptions);
		
		//Adding the markers
		placeMarkers(genome,mymap);
		
		//Drawing the lines
		var linepoints = new Array();
		for(var i in genome)
			linepoints.push(new google.maps.LatLng(genome[i].lat,genome[i].lon))
		var line = new google.maps.Polyline({map:mymap,path:linepoints,strokeColor:"yellow"});
	}
		 
	//Placing the markers on the map
	function placeMarkers(markers,mymap){
		//Looping through the array of markers
		var bounds = new google.maps.LatLngBounds();
		for( var i in markers )	{
			//Creating the actual marker object
			var latlng = new google.maps.LatLng(parseFloat(markers[i].lat), parseFloat(markers[i].lon));
			bounds.extend(latlng);
			createMarker(markers[i],mymap,i);
		}		
		//Fitting the map to the markers
		mymap.fitBounds(bounds);
	}
 
	//Creating An Individual Marker 
	var infowindow = false;
	function createMarker(markerData,mymap,iter) {
		
		//Picking the color
		var color = "5bff45";
		var z = 0;
		if( iter == 0 ){ color = "ff6565"; z = 500; }	//setting the color and z index for the first pin
		
		//Creating the marker image				
		var latlng = new google.maps.LatLng(parseFloat(markerData.lat), parseFloat(markerData.lon));
		//var mark_img = new google.maps.MarkerImage("http://www.google.com/intl/en_us/mapfiles/ms/micons/"+color+"-dot.png");
		mark_img = "http://chart.apis.google.com/chart?chst=d_map_pin_letter&chld="+iter+"|"+color+"|000000";
		
		//Creating the marker object
		var mymarker = new google.maps.Marker({
			position: latlng, 
			map: mymap,
			draggable: false,
			title:markerData.title,
			icon: mark_img,
			zIndex:z
		});
		
		//Adding a listener
		google.maps.event.addListener(mymarker, 'click', function() {});
	}


var map = L.map('map', {
    center: [22.604799, 120.2976256],
    zoom: 16,
    minZoom:8
});

// 初始圖層
var darkLayer = L.tileLayer('https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}{r}.png', {
  attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors &copy; <a href="https://carto.com/attributions">CARTO</a>',
  subdomains: 'abcd'
}).addTo(map);

var osmLayer = L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
  attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
});

// 新增控制器
var baseMaps = {
  "暗黑模式": darkLayer,
  "明亮模式": osmLayer
};

L.control.layers(baseMaps).addTo(map);




var data = [
    { 'name': '軟體園區', lat: 22.604799, lng: 120.2976256, price: 100, type: 'home', scale: 5, site: '新北市' },
    { 'name': 'ikea', lat: 22.6066728, lng: 120.3015429, price: 500, type: 'home', scale: 10, site: '台北市' }
];

var markers = new L.MarkerClusterGroup().addTo(map);

for (let i = 0; i < data.length; i++) {
    let popupContent = `
        <div class="details">
            <div class="price">原價$${data[i].price}</div>
            <div class="address">${data[i].name}</div>
            <div class="features">
                <div>
                    <i aria-hidden="true" class="fa-solid fa-cart-shopping" title="scale"></i>
                    <span class="fa-sr-only">scale</span>
                    <span>已銷售${data[i].scale}</span>
                </div>
                <div>
                    <i aria-hidden="true" class="fa-solid fa-location-dot" title="site"></i>
                    <span class="fa-sr-only">site</span>
                    <span>${data[i].site}</span>
                </div>
            </div>
        </div>
    `;

    var customIcon = L.divIcon({
        className: 'property',
        html: `
            <div class="icon">
                <i aria-hidden="true" class="fas fa-${data[i].type}" title="${data[i].type}"></i>
                <span class="fa-sr-only">${data[i].type}</span>
            </div>
        `,
        iconSize: [30, 30], // 圖示的大小
        iconAnchor: [15, 15] // 圖示的位置
    });

    var marker = L.marker([data[i].lat, data[i].lng], {icon: customIcon}).addTo(map);
    marker.bindPopup(popupContent);

    marker.on('mouseover', function (e) {
        this.bindPopup(popupContent).openPopup();
    });

    marker.on('mouseout', function (e) {
        this.closePopup();
    });
    
    markers.addLayer(marker);
}

map.addLayer(markers);
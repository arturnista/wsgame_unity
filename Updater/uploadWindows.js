const path = require('path')
const firebase = require('firebase')
const googleStorage = require('@google-cloud/storage')

const storage = googleStorage({
    projectId: 'nwgame-d8f9d',
    keyFilename: './nwgame-key.json'
})

const bucket = storage.bucket('nwgame-d8f9d.appspot.com')
const bucketName = 'nwgame-d8f9d.appspot.com'

function uploadFile(filename, origin, destination) {
    return storage
    .bucket(bucketName)
    .upload(origin + '/' + filename)

    .then(() =>
        storage
        .bucket(bucketName)
        .file(filename)
        .move(destination + '/' + filename)
    )
    .catch(err => {
        console.error('ERROR:', err);
    })
}


function uploadFiles(files) {
    console.log(`=== UPLOADING ${files.length} FILES`)
    files = files.map(f => ({
        name: path.basename(f),
        origin: path.dirname('./Build/' + f),
        destination: path.dirname(f)
    }))

    return files.reduce((prev, file, index) => {
        console.log(`File ${index} - ${file.origin}/${file.name}`)
        return prev.then(() => uploadFile(file.name, file.origin, file.destination))
    }, Promise.resolve())
}

const files = [
    'Windows/NW_Data/level0',
    'Windows/NW_Data/level1',
    'Windows/NW_Data/sharedassets1.assets',
    'Windows/NW_Data/sharedassets0.assets.resS',
    'Windows/NW_Data/globalgamemanagers',
    'Windows/NW_Data/level2',
    'Windows/NW_Data/sharedassets1.assets.resS',
    'Windows/NW_Data/sharedassets2.assets.resS',
    'Windows/NW_Data/sharedassets2.assets',
    'Windows/NW_Data/sharedassets0.assets',
    'Windows/NW_Data/globalgamemanagers.assets',
    'Windows/NW_Data/boot.config',
    'Windows/NW_Data/Resources/unity default resources',
    'Windows/NW_Data/Resources/unity_builtin_extra',
    'Windows/NW_Data/Mono/EmbedRuntime/MonoPosixHelper.dll',
    'Windows/NW_Data/Mono/EmbedRuntime/mono.dll',
    'Windows/NW_Data/Mono/etc/mono/1.0/machine.config',
    'Windows/NW_Data/Mono/etc/mono/1.0/DefaultWsdlHelpGenerator.aspx',
    'Windows/NW_Data/Mono/etc/mono/mconfig/config.xml',
    'Windows/NW_Data/Mono/etc/mono/browscap.ini',
    'Windows/NW_Data/Mono/etc/mono/2.0/machine.config',
    'Windows/NW_Data/Mono/etc/mono/2.0/settings.map',
    'Windows/NW_Data/Mono/etc/mono/2.0/web.config',
    'Windows/NW_Data/Mono/etc/mono/2.0/Browsers/Compat.browser.html',
    'Windows/NW_Data/Mono/etc/mono/2.0/DefaultWsdlHelpGenerator.aspx',
    'Windows/NW_Data/Mono/etc/mono/config',
    'Windows/NW_Data/Managed/UnityEngine.Timeline.dll',
    'Windows/NW_Data/Managed/UnityEngine.UnityWebRequestAudioModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.SpatialTracking.dll',
    'Windows/NW_Data/Managed/UnityEngine.PhysicsModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.StyleSheetsModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.Networking.dll',
    'Windows/NW_Data/Managed/UnityEngine.UNETModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.UnityWebRequestModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.ImageConversionModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.ParticleSystemModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.UnityWebRequestWWWModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.ClusterInputModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.AnimationModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.TerrainPhysicsModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.WindModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.AIModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.GridModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.UI.dll',
    'Windows/NW_Data/Managed/System.Core.dll',
    'Windows/NW_Data/Managed/UnityEngine.DirectorModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.CoreModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.UnityWebRequestTextureModule.dll',
    'Windows/NW_Data/Managed/Assembly-CSharp.dll',
    'Windows/NW_Data/Managed/UnityEngine.TerrainModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.ScreenCaptureModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.TilemapModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.AccessibilityModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.VideoModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.GameCenterModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.UnityConnectModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.SpriteShapeModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.VehiclesModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.ClothModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.ParticlesLegacyModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.dll',
    'Windows/NW_Data/Managed/UnityEngine.WebModule.dll',
    'Windows/NW_Data/Managed/Mono.Security.dll',
    'Windows/NW_Data/Managed/UnityEngine.SharedInternalsModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.PerformanceReportingModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.ARModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.AudioModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.UnityAnalyticsModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.VRModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.TextRenderingModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.Analytics.dll',
    'Windows/NW_Data/Managed/UnityEngine.Physics2DModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.UIElementsModule.dll',
    'Windows/NW_Data/Managed/System.dll',
    'Windows/NW_Data/Managed/UnityEngine.UIModule.dll',
    'Windows/NW_Data/Managed/mscorlib.dll',
    'Windows/NW_Data/Managed/UnityEngine.ClusterRendererModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.IMGUIModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.AssetBundleModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.JSONSerializeModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.SpriteMaskModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.CrashReportingModule.dll',
    'Windows/NW_Data/Managed/UnityEngine.InputModule.dll',
]
uploadFiles(files)
.then(() => {
    console.log('UPLOAD FINAL')
})

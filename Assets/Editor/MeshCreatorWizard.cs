using UnityEngine;
using System.Collections;
using UnityEditor;

// enum for the dropdown object type selector
public enum ObjectMeshType
{
    Flat2D = 0,
    Full3D = 1
}

public enum ObjectColliderType
{
    Boxes = 0,
    Mesh = 1,
    // AABB = 2, 
    None = 3
}

// thanks to Chris Reilly for changing this to EditorWindow from Wizard
public class MeshCreatorWizard : EditorWindow {
	private const float versionNumber = 0.6f;
	private Texture2D gameLabLogo = Resources.Load("games.ucla.logo.small") as Texture2D;
	public Texture2D textureToCreateMeshFrom;

	public bool withColliders;
	public float xWidth = 1.0f;
	public float yHeight = 1.0f;
	public float zDepth = 1.0f;
	public string gameObjectName = "Mesh Creator Object";

    // enum for the meshtype(2d,3d) to be created
    public ObjectMeshType meshType = ObjectMeshType.Flat2D;

    // enum for they collider type to be created
    public ObjectColliderType colliderType = ObjectColliderType.Boxes;
	
    // window size
    static public Vector2 minWindowSize = new Vector2(600, 425);
	
	// Add menu named "Create Mesh Object" to the GameObject menu
	[MenuItem ("GameObject/Create Mesh Object")]
	
	static void Init () {
		// Get existing open window or if none, make a new one:
		MeshCreatorWizard window = (MeshCreatorWizard)EditorWindow.GetWindow( typeof( MeshCreatorWizard ), true, "Create Mesh Object v" + versionNumber );
        window.minSize = minWindowSize;
	}
	
	void OnGUI () {
		
		EditorGUIUtility.AddCursorRect( new Rect(10,10,400,150), MouseCursor.Link); 
		
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			
			// display game lab logo & link 
			if( GUILayout.Button( gameLabLogo ) ) {
				Application.OpenURL ("http://games.ucla.edu/");	
			}
					
			GUILayout.FlexibleSpace();
				//basic instructions
				GUILayout.Label( "Choose a texture with alpha channel to create a mesh from\nSquare images are recommended.\n\nThen select whether to create depth on the mesh and whether you\nwant colliders for your new mesh.\n\nEnter a game object name and you are good to go.\n\nAdvanced control is available once you create the object.", GUILayout.Width(400) );
			GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
        EditorGUILayout.Space();
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			
			GUILayout.BeginVertical();			
			//source texture
            EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
				GUILayout.Label( "Texture to Create Mesh From", GUILayout.Width(175) );
                GUILayoutOption[] textureDisplaySize = { GUILayout.Width(150), GUILayout.Height(150)};
                textureToCreateMeshFrom = (Texture2D)EditorGUILayout.ObjectField(textureToCreateMeshFrom, typeof(Texture2D), false, textureDisplaySize);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			// what type of object being created, 2d or 3d?
            GUILayout.BeginHorizontal();
            meshType = (ObjectMeshType) EditorGUILayout.EnumPopup("Mesh Type", meshType, GUILayout.Width(330));
            GUILayout.EndHorizontal();
			
			//with colliders?
			GUILayout.BeginHorizontal();
            colliderType = (ObjectColliderType)EditorGUILayout.EnumPopup("Collider Type", colliderType, GUILayout.Width(330));
			GUILayout.EndHorizontal();
			
			//object name
			GUILayout.BeginHorizontal();
				GUILayout.Label( "Game Object Name", GUILayout.Width(175) );
				gameObjectName = GUILayout.TextField( gameObjectName, 50, GUILayout.Width(175) );
			GUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
			//submit button
			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if( GUILayout.Button( "Create Mesh", GUILayout.Width(100) ) ){
					// create the new object and set the proper variables		
					GameObject newObject = new GameObject(gameObjectName);
					MeshCreatorData mcd = newObject.AddComponent( "MeshCreatorData" ) as MeshCreatorData;
					
					// set up mcd
					mcd.outlineTexture = textureToCreateMeshFrom;
					mcd.useAutoGeneratedMaterial = true;
					mcd.meshHeight = yHeight;
					mcd.meshWidth = xWidth;
					mcd.meshDepth = zDepth;
					
					// set up the depth options
					if (meshType == ObjectMeshType.Full3D)
					{
						mcd.uvWrapMesh = true;
						mcd.createEdges = false;
						mcd.createBacksidePlane = false;
					}
					else
					{
						mcd.uvWrapMesh = false;
						mcd.createEdges = false;
						mcd.createBacksidePlane = false;
					}
					
					// set up the collider options
					if (colliderType == ObjectColliderType.Boxes)
					{
						mcd.generateCollider = true;
						mcd.usePrimitiveCollider = true;
                        mcd.maxNumberBoxes = 20;
						mcd.usePhysicMaterial = false;
						mcd.addRigidBody = false;
					}
                    else if (colliderType == ObjectColliderType.Mesh)
                    {
                        mcd.generateCollider = true;
                        mcd.usePrimitiveCollider = false;
                        mcd.maxNumberBoxes = 20;
                        mcd.usePhysicMaterial = false;
                        mcd.addRigidBody = false;
                    }
                    else // default to none
                    {
                        mcd.generateCollider = false;
                        mcd.usePrimitiveCollider = false;
                        mcd.maxNumberBoxes = 20;
                        mcd.usePhysicMaterial = false;
                        mcd.addRigidBody = false;
                    }
					
					// update the mesh
					MeshCreator.UpdateMesh(newObject);
					Close();
				}
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();	
			
			GUILayout.EndVertical();
			
		GUILayout.EndHorizontal();
		
	}
}

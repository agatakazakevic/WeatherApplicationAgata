package crc64c878015280917d3d;


public class forecastadapterViewHolder
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("weatherapplication.forecastadapterViewHolder, weatherapplication", forecastadapterViewHolder.class, __md_methods);
	}


	public forecastadapterViewHolder ()
	{
		super ();
		if (getClass () == forecastadapterViewHolder.class)
			mono.android.TypeManager.Activate ("weatherapplication.forecastadapterViewHolder, weatherapplication", "", this, new java.lang.Object[] {  });
	}

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
